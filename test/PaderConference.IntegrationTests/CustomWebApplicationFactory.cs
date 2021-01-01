using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Auth.AuthService;

namespace PaderConference.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public const string USERNAME = "Vincent";
        public const string PASSWORD = "123";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
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
                });
        }
    }
}

