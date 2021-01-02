using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mongo2Go;
using Moq;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Auth.AuthService;
using PaderConference.Infrastructure.Data;

namespace PaderConference.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public const string USERNAME = "Vincent";
        public const string PASSWORD = "123";
        private MongoDbRunner? _runner;

        protected override void Dispose(bool disposing)
        {
            _runner?.Dispose();
            base.Dispose(disposing);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var runner = MongoDbRunner.Start();
            _runner = runner;

            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IAuthService>(new OptionsAuthService(
                    new OptionsWrapper<UserCredentialsOptions>(new UserCredentialsOptions
                    {
                        Users = new Dictionary<string, OptionsUserData>
                        {
                            {
                                USERNAME,
                                new OptionsUserData {DisplayName = "Vincent", Id = "123", Password = PASSWORD}
                            },
                        },
                    })));

                services.AddSingleton<IOptions<MongoDbOptions>>(new OptionsWrapper<MongoDbOptions>(new MongoDbOptions
                {
                    DatabaseName = Guid.NewGuid().ToString("N"), ConnectionString = runner.ConnectionString,
                }));

                services.AddSingleton(new Mock<IOpenConferenceRepo>().Object);
            });
        }
    }
}

