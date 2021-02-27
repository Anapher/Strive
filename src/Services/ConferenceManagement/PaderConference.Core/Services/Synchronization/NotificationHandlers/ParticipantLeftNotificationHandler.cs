using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;

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
            var (participant, _) = notification;

            var removedSubscriptions = await _subscriptionsRepository.Remove(participant);
            if (removedSubscriptions?.Any() == true)
                await _mediator.Publish(
                    new ParticipantSubscriptionsRemovedNotification(participant, removedSubscriptions),
                    cancellationToken);
        }
    }
}
