using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Synchronization.NotificationHandlers
{
    public class ParticipantLeftNotificationHandler : INotificationHandler<ParticipantLeftNotification>
    {
        private readonly IMediator _mediator;
        private readonly ISynchronizedObjectSubscriptionsRepository _subscriptionsRepository;

        public ParticipantLeftNotificationHandler(IMediator mediator,
            ISynchronizedObjectSubscriptionsRepository subscriptionsRepository)
        {
            _mediator = mediator;
            _subscriptionsRepository = subscriptionsRepository;
        }

        public async Task Handle(ParticipantLeftNotification notification, CancellationToken cancellationToken)
        {
            var (participantId, conferenceId, _) = notification;

            var removedSubscriptions = await _subscriptionsRepository.Remove(conferenceId, participantId);
            if (removedSubscriptions?.Any() == true)
                await _mediator.Publish(
                    new ParticipantSubscriptionsRemovedNotification(conferenceId, participantId, removedSubscriptions),
                    cancellationToken);
        }
    }
}
