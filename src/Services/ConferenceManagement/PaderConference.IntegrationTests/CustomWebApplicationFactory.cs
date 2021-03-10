using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using PaderConference.IntegrationTests._Helpers;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly MongoDbFixture _mongoDb;

        public CustomWebApplicationFactory(MongoDbFixture mongoDb, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _mongoDb = mongoDb;
        }

        public MockJwtTokens JwtTokens { get; } = new();

        public UserAccount CreateUser(string name, bool isModerator)
        {
            var sub = Guid.NewGuid().ToString("N");

            var claims = new List<Claim> {new(ClaimTypes.NameIdentifier, sub), new("name", name)};
            if (isModerator)
                claims.Add(new Claim(ClaimTypes.Role, AppRoles.Moderator));

            var token = JwtTokens.GenerateJwtToken(claims);
            return new UserAccount(sub, name, isModerator, token);
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return base.CreateWebHostBuilder().UseSerilog(_testOutputHelper.CreateTestLogger(LogEventLevel.Debug));
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var configuration = StartMongoDbAndGetConfiguration();

            builder.ConfigureAppConfiguration(configurationBuilder =>
                configurationBuilder.AddConfiguration(configuration));

            builder.ConfigureServices(services =>
            {
                services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var config = new OpenIdConnectConfiguration {Issuer = JwtTokens.Issuer};

                    config.SigningKeys.Add(JwtTokens.SecurityKey);
                    options.Configuration = config;
                });
            });
        }

        private IConfiguration StartMongoDbAndGetConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(new EmbeddedFileProvider(typeof(CustomWebApplicationFactory).Assembly),
                    "appsettings.IntegrationTest.json", false, false).Build();

            configuration["MongoDb:ConnectionString"] = _mongoDb.Runner.ConnectionString;
            return configuration;
        }
    }

    public record UserAccount(string Sub, string Name, bool IsModerator, string Token)
    {
        public UserAccount SetupHttpClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, Token);
            return this;
        }
    }
}
