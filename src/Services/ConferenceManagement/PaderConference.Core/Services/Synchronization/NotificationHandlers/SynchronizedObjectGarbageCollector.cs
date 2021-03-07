using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;

namespace PaderConference.Core.Services.Synchronization.NotificationHandlers
{
    public class SynchronizedObjectGarbageCollector : INotificationHandler<ParticipantSubscriptionsUpdatedNotification>
    {
        private readonly ISynchronizedObjectRepository _synchronizedObjectRepository;
        private readonly ISynchronizedObjectSubscriptionsRepository _subscriptionsRepository;

        public SynchronizedObjectGarbageCollector(ISynchronizedObjectRepository synchronizedObjectRepository,
            ISynchronizedObjectSubscriptionsRepository subscriptionsRepository)
        {
            _synchronizedObjectRepository = synchronizedObjectRepository;
            _subscriptionsRepository = subscriptionsRepository;
        }

        public async Task Handle(ParticipantSubscriptionsUpdatedNotification notification,
            CancellationToken cancellationToken)
        {
            var ((conferenceId, _), removedSubscriptions, _) = notification;

            if (!removedSubscriptions.Any()) return;

            var conferenceSubscriptions = await _subscriptionsRepository.GetOfConference(conferenceId);
            var activeSyncObjects = conferenceSubscriptions.SelectMany(x => x.Value ?? ImmutableList<string>.Empty)
                .ToHashSet();

            var inactiveSyncObjects = removedSubscriptions.Select(x => x.ToString()).Except(activeSyncObjects);
            foreach (var inactiveSubscription in inactiveSyncObjects)
            {
                await _synchronizedObjectRepository.Remove(conferenceId, inactiveSubscription);
            }
        }
    }
}
