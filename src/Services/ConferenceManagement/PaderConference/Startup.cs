using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PaderConference.Config;
using PaderConference.Core;
using PaderConference.Core.Domain.Entities;
using PaderConference.Extensions;
using PaderConference.Hubs;
using PaderConference.Infrastructure;
using PaderConference.Infrastructure.Data;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Impl;
using PaderConference.Infrastructure.Redis.InMemory;
using PaderConference.Infrastructure.Serialization;
using PaderConference.Messaging.Consumers;
using PaderConference.Services;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using Swashbuckle.AspNetCore.Swagger;

namespace PaderConference
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        static Startup()
        {
            BsonSerializer.RegisterGenericSerializerDefinition(typeof(IImmutableList<>),
                typeof(ImmutableListSerializer<>));
            BsonSerializer.RegisterGenericSerializerDefinition(typeof(IImmutableDictionary<,>),
                typeof(ImmutableDictionarySerializer<,>));
            BsonSerializer.RegisterSerializer(new JTokenBsonSerializer());
            BsonSerializer.RegisterSerializer(new EnumSerializer<PermissionType>(BsonType.String));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            // Authentication
            var authOptions = Configuration.GetSection("Authentication").Get<AuthOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = authOptions.Authority;
                    options.TokenValidationParameters = new TokenValidationParameters {ValidateAudience = false};
#if DEBUG
                    options.RequireHttpsMetadata = false;
#endif

                    options.AcceptTokenFromQuery();
                });

            // SignalR
            services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
            {
                options.PayloadSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.PayloadSerializerSettings.Converters.Add(
                    new StringEnumConverter(new CamelCaseNamingStrategy()));
            });

            services.AddMvc().ConfigureApiBehaviorOptions(options => options.UseInvalidModelStateToError())
                .AddFluentValidation(fv =>
                    fv.RegisterValidatorsFromAssemblyContaining<Startup>()
                        .RegisterValidatorsFromAssemblyContaining<CoreModule>()).AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(CoreModule).Assembly);

            var healthChecks = services.AddHealthChecks();

            // KeyValuDatabase
            var keyValueOptions = Configuration.GetSection("KeyValueDatabase").Get<KeyValueDatabaseOptions>();
            if (keyValueOptions.UseInMemory)
            {
                services.AddSingleton<IKeyValueDatabase, InMemoryKeyValueDatabase>(_ =>
                    new InMemoryKeyValueDatabase(new InMemoryKeyValueData()));
            }
            else
            {
                var config = keyValueOptions.Redis ?? new RedisConfiguration();
                services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(config);
                services.AddSingleton(s => s.GetRequiredService<IRedisDatabase>().Database);
                services.AddSingleton<IKeyValueDatabase, RedisKeyValueDatabase>();

                healthChecks.AddRedis(config.ConnectionString);
            }

            // MongoDb
            services.Configure<MongoDbOptions>(Configuration.GetSection("MongoDb"));
            services.AddHostedService<MongoDbBuilder>();

            var mongoOptions = Configuration.GetSection("MongoDb").Get<MongoDbOptions>();
            healthChecks.AddMongoDb(mongoOptions.ConnectionString);

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Predicate = check => check.Tags.Contains("ready");
            });

            // RabbitMq
            var rabbitMqOptions = Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>();
            services.AddMassTransit(x =>
            {
                //x.AddSignalRHub<CoreHub>();

                x.AddConsumersFromNamespaceContaining<ParticipantKickedConsumer>();

                if (rabbitMqOptions.UseInMemory)
                    x.UsingInMemory();
                else
                    x.UsingRabbitMq((context, configurator) =>
                    {
                        if (rabbitMqOptions.RabbitMq != null)
                            configurator.ConfigureOptions(rabbitMqOptions.RabbitMq);

                        configurator.UseHealthCheck(context);
                        configurator.ConfigureEndpoints(context);
                    });
            });
            services.AddMassTransitHostedService();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Pader Conference API", Version = "v1"});

                var scheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                };

                // Swagger 2.+ support
                c.AddSecurityDefinition("Bearer", scheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {{scheme, new List<string>()}});

                c.AddFluentValidationRules();
            });

            services.AddMediatR(typeof(Startup), typeof(CoreModule));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });

            // Now register our services with Autofac container.
            var builder = new ContainerBuilder();

            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new InfrastructureModule());
            builder.RegisterModule(new PresentationModule());

            builder.Populate(services);
            var container = builder.Build();
            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseCors("AllowAll");

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pader Conference API V1"));

            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<CoreHub>("/signalr");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/ready",
                    new HealthCheckOptions {Predicate = check => check.Tags.Contains("ready")});

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions());
            });
        }
    }
}