using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.IntegrationTests.Mock;

namespace PaderConference.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IConferenceRepo, ConferenceRepoMock>();
                //var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDocumentStore));
                //if (descriptor != null)
                //    services.Remove(descriptor);
            });
        }
    }
}