using MassTransit;
using MassTransit.RabbitMqTransport;
using PaderConference.Config;

namespace PaderConference.Extensions
{
    public static class RabbitMqExtensions
    {
        public static void ConfigureOptions(this IRabbitMqBusFactoryConfigurator configurator,
            RabbitMqHostOptions options)
        {
            configurator.Host(options.Host, c =>
            {
                if (options.Username != null)
                    c.Username(options.Username);
                if (options.Password != null)
                    c.Password(options.Password);
            });
        }
    }
}
