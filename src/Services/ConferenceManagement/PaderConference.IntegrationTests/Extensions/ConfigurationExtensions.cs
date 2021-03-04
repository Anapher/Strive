using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PaderConference.IntegrationTests.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void OverwriteConfiguration<T>(this IServiceCollection services, T options) where T : class
        {
            services.AddSingleton<IOptions<T>>(new OptionsWrapper<T>(options));
        }
    }
}
