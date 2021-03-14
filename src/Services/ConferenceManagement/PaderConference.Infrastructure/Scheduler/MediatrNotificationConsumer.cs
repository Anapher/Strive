using System.Threading.Tasks;
using MassTransit;
using MassTransit.Scheduling;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Interfaces;

namespace PaderConference.Infrastructure.Scheduler
{
    public class MediatrNotificationConsumer : IConsumer<IScheduledNotification>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MediatrNotificationConsumer> _logger;

        public MediatrNotificationConsumer(IMediator mediator, ILogger<MediatrNotificationConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IScheduledNotification> context)
        {
            var token = context.GetSchedulingTokenId();
            if (token == null)
                _logger.LogWarning("A scheduled notification {notification} was received, but the token id is null.",
                    context.Message);

            var tokenId = token?.ToString("N");

            var message = context.Message;
            message.TokenId = tokenId;

            return _mediator.Publish(message, context.CancellationToken);
        }
    }
}
