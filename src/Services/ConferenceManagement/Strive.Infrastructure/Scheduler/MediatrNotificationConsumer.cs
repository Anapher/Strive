using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Scheduling;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Strive.Core.Interfaces;

namespace Strive.Infrastructure.Scheduler
{
    public class MediatrNotificationConsumer : IConsumer<IScheduledNotificationWrapper>
    {
        private readonly ILogger<MediatrNotificationConsumer> _logger;
        private readonly IMediator _mediator;

        public MediatrNotificationConsumer(IMediator mediator, ILogger<MediatrNotificationConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IScheduledNotificationWrapper> context)
        {
            _logger.LogDebug("Received scheduled notification of type {typeName}: {message}", context.Message.TypeName,
                context.Message.JsonSerialized);

            var token = context.GetSchedulingTokenId();
            if (token == null)
                _logger.LogWarning("A scheduled notification {notification} was received, but the token id is null.",
                    context.Message);

            var type = Type.GetType(context.Message.TypeName);
            if (type == null)
            {
                _logger.LogWarning("The type {typeName} was not found, cannot process notification.",
                    context.Message.TypeName);
                return;
            }

            var notification =
                (IScheduledNotification?) JsonConvert.DeserializeObject(context.Message.JsonSerialized, type);
            if (notification == null)
            {
                _logger.LogError("Received null notification");
                return;
            }

            var tokenId = token?.ToString("N");
            notification.TokenId = tokenId;

            await _mediator.Publish(notification, context.CancellationToken);
        }
    }
}
