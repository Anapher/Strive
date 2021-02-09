using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation.AspNetCore;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PaderConference.Auth;
using PaderConference.Consumers;
using PaderConference.Core;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Errors;
using PaderConference.Hubs;
using PaderConference.Infrastructure;
using PaderConference.Infrastructure.Data;
using PaderConference.Infrastructure.Redis.Extensions;
using PaderConference.Infrastructure.Serialization;
using PaderConference.Services;
using StackExchange.Redis.Extensions.Core.Configuration;
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

            // Configure MongoDb options
            services.Configure<MongoDbOptions>(Configuration.GetSection("MongoDb"));

            // add MongoDb
            services.AddHostedService<MongoDbBuilder>();

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme,
                        EquipmentAuthExtensions.EquipmentAuthScheme).Build();
            });

            services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
            {
                options.PayloadSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.PayloadSerializerSettings.Converters.Add(
                    new StringEnumConverter(new CamelCaseNamingStrategy()));

                //options.PayloadSerializerSettings.Converters.Add(JsonSubtypesConverterBuilder
                //    .Of<SendingMode>(nameof(SendingMode.Type)).RegisterSubtype<SendAnonymously>(SendAnonymously.TYPE)
                //    .RegisterSubtype<SendPrivately>(SendPrivately.TYPE).SerializeDiscriminatorProperty().Build());
            });

            services.AddMvc().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                    new BadRequestObjectResult(new FieldValidationError(context.ModelState
                        .Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                        .ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage)));
            }).AddFluentValidation(fv =>
                fv.RegisterValidatorsFromAssemblyContaining<Startup>()
                    .RegisterValidatorsFromAssemblyContaining<CoreModule>()).AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(CoreModule).Assembly);

            var redisConfig = Configuration.GetSection("Redis").Get<RedisConfiguration>() ?? new RedisConfiguration();
            services.AddStackExchangeRedisExtensions<CamelCaseNewtonSerializer>(redisConfig);

            services.AddHealthChecks().AddMongoDb(Configuration["MongoDb:ConnectionString"]);

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Predicate = check => check.Tags.Contains("ready");
            });

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq();
                x.AddConsumersFromNamespaceContaining<ParticipantKickedNotificationConsumer>();
            });
            services.AddMassTransitHostedService();

            // Register the Swagger generator, defining 1 or more Swagger documents
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