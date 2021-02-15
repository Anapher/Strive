using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.ConferenceControl.NotificationHandlers
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

            _logger.LogDebug("Conference {conferenceId} updated, update synchronized object", conferenceId);

            await _mediator.Send(
                new UpdateSynchronizedObjectRequest(conferenceId,
                    SynchronizedConferenceInfoProvider.SynchronizedObjectId), cancellationToken);
        }
    }
}
