using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PaderConference.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDocumentStore));
                //if (descriptor != null)
                //    services.Remove(descriptor);
            });
        }
    }
}