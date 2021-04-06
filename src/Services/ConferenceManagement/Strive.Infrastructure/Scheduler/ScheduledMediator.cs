using System;
using System.Threading.Tasks;
using MassTransit;
using Newtonsoft.Json;
using Strive.Core.Interfaces;
using Strive.Core.Interfaces.Services;

namespace Strive.Infrastructure.Scheduler
{
    public class ScheduledMediator : IScheduledMediator
    {
        private const string QueueName = "notification-scheduler";
        private static readonly Uri Destination = new($"queue:{QueueName}");

        private readonly IMessageScheduler _messageScheduler;

        public ScheduledMediator(IMessageScheduler messageScheduler)
        {
            _messageScheduler = messageScheduler;
        }

        public static void Configure(IReceiveConfigurator configurator, IServiceProvider context)
        {
            configurator.ReceiveEndpoint(QueueName, e => e.Consumer<MediatrNotificationConsumer>(context));
        }

        public async ValueTask<string> Schedule<T>(T notification, DateTimeOffset scheduleDate)
            where T : class, IScheduledNotification
        {
            var serialized = JsonConvert.SerializeObject(notification);
            var typeName = typeof(T).AssemblyQualifiedName;

            var message = await _messageScheduler.ScheduleSend<IScheduledNotificationWrapper>(Destination,
                scheduleDate.UtcDateTime, new {JsonSerialized = serialized, TypeName = typeName});

            return message.TokenId.ToString("N");
        }

        public async ValueTask Remove<T>(string id) where T : class, IScheduledNotification
        {
            await _messageScheduler.CancelScheduledSend(Destination, Guid.ParseExact(id, "N"));
        }
    }
}
