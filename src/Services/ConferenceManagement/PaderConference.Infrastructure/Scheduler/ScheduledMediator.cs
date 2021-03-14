using System;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Infrastructure.Scheduler
{
    public class ScheduledMediator : IScheduledMediator
    {
        private readonly IMessageScheduler _messageScheduler;

        public ScheduledMediator(IMessageScheduler messageScheduler)
        {
            _messageScheduler = messageScheduler;
        }

        public async ValueTask<string> Schedule<T>(T notification, DateTimeOffset scheduleDate)
            where T : class, INotification
        {
            var message = await _messageScheduler.SchedulePublish(scheduleDate.DateTime, notification);
            return message.TokenId.ToString("N");
        }

        public async ValueTask Remove<T>(string id) where T : class, INotification
        {
            await _messageScheduler.CancelScheduledPublish<T>(Guid.ParseExact(id, "N"));
        }
    }
}
