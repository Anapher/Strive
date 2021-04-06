using MassTransit.RabbitMqTransport;
using Strive.Config;
using RabbitMQ.Client;

namespace Strive.Extensions
{
    public static class RabbitMqConfiguratorExtensions
    {
        public static void ConfigurePublishMessage<T>(this IRabbitMqBusFactoryConfigurator configurator,
            SfuOptions options) where T : class
        {
            configurator.Message<T>(topologyConfigurator =>
            {
                topologyConfigurator.SetEntityName(options.PublishExchange);
            });
            configurator.Publish<T>(top =>
            {
                top.ExchangeType = ExchangeType.Direct;
                top.Durable = false;
            });
        }
    }
}
