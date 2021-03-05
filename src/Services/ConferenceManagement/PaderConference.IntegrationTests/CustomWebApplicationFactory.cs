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
using Mongo2Go;
using Mongo2Go.Helper;
using PaderConference.IntegrationTests._Helpers;

namespace PaderConference.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private MongoDbRunner? _runner;

        protected override void Dispose(bool disposing)
        {
            _runner?.Dispose();
            base.Dispose(disposing);
        }

        public MockJwtTokens JwtTokens { get; } = new();

        public UserAccount CreateUser(string name, bool isModerator)
        {
            var sub = Guid.NewGuid().ToString("N");

            var claims = new List<Claim> {new(ClaimTypes.NameIdentifier, sub), new(ClaimTypes.Name, name)};
            if (isModerator)
                claims.Add(new Claim(ClaimTypes.Role, AppRoles.Moderator));

            var token = JwtTokens.GenerateJwtToken(claims);
            return new UserAccount(sub, name, isModerator, token);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            _runner = MongoDbRunner.StartUnitTest(Mongo2GoPortPool.Instance, new FileSystem(),
                new MongoDbProcessStarter(), new MongoBinaryLocator(null, null));
#pragma warning restore CS0618 // Type or member is obsolete

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(new EmbeddedFileProvider(typeof(CustomWebApplicationFactory).Assembly),
                    "appsettings.IntegrationTest.json", false, false).Build();

            configuration["MongoDb:ConnectionString"] = _runner.ConnectionString;

            builder.UseConfiguration(configuration);

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
