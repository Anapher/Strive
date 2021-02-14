using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.ConferenceControl.NotificationHandlers
{
    public class ConferenceStateUpdatedNotificationHandler : INotificationHandler<ConferenceStateUpdatedNotification>
    {
        private readonly IMediator _mediator;
        private readonly IParticipantAggregator _participantAggregator;
        private readonly ILogger<ConferenceStateUpdatedNotificationHandler> _logger;

        public ConferenceStateUpdatedNotificationHandler(IMediator mediator,
            IParticipantAggregator participantAggregator, ILogger<ConferenceStateUpdatedNotificationHandler> logger)
        {
            _mediator = mediator;
            _participantAggregator = participantAggregator;
            _logger = logger;
        }

        public async Task Handle(ConferenceStateUpdatedNotification notification, CancellationToken cancellationToken)
        {
            var conferenceId = notification.ConferenceId;

            _logger.LogDebug("Conference {conferenceId} updated, update synchronized object", conferenceId);

            var participants = await _participantAggregator.OfConference(conferenceId);
            await _mediator.Send(
                UpdateSynchronizedObjectRequest.Create<SynchronizedConferenceInfoProvider>(conferenceId, participants),
                cancellationToken);
        }
    }
}
