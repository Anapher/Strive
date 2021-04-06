using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.ConferenceControl.NotificationHandlers
{
    public class ConferenceStateUpdatedNotificationHandler : INotificationHandler<ConferenceStateUpdatedNotification>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ConferenceStateUpdatedNotificationHandler> _logger;

        public ConferenceStateUpdatedNotificationHandler(IMediator mediator,
            ILogger<ConferenceStateUpdatedNotificationHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(ConferenceStateUpdatedNotification notification, CancellationToken cancellationToken)
        {
            var conferenceId = notification.ConferenceId;

            _logger.LogDebug("Conference {conferenceId} state updated, update synchronized object", conferenceId);

            await _mediator.Send(
                new UpdateSynchronizedObjectRequest(conferenceId, SynchronizedConferenceInfo.SyncObjId),
                cancellationToken);
        }
    }
}
