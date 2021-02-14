using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Synchronization.UseCases
{
    public class UpdateSynchronizedObjectUseCase : IRequestHandler<UpdateSynchronizedObjectRequest>
    {
        private readonly IComponentContext _context;
        private readonly ISynchronizedObjectRepository _synchronizedObjectRepository;
        private readonly ISynchronizedObjectSubscriptionsRepository _subscriptionsRepository;
        private readonly IMediator _mediator;

        private readonly Dictionary<string, SynchronizedObjectUpdatedNotification> _cachedSyncObjectNotifications =
            new();

        public UpdateSynchronizedObjectUseCase(IComponentContext context,
            ISynchronizedObjectRepository synchronizedObjectRepository,
            ISynchronizedObjectSubscriptionsRepository subscriptionsRepository, IMediator mediator)
        {
            _context = context;
            _synchronizedObjectRepository = synchronizedObjectRepository;
            _subscriptionsRepository = subscriptionsRepository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UpdateSynchronizedObjectRequest request, CancellationToken cancellationToken)
        {
            var provider = (ISynchronizedObjectProvider) _context.Resolve(request.ProviderType);

            foreach (var participantId in request.ParticipantIds)
            {
                await UpdateSynchronizedObject(request.ConferenceId, participantId, provider);
            }

            foreach (var notification in _cachedSyncObjectNotifications.Values)
            {
                await _mediator.Publish(notification);
            }

            return Unit.Value;
        }

        private async ValueTask UpdateSynchronizedObject(string conferenceId, string participantId,
            ISynchronizedObjectProvider provider)
        {
            var syncObjId = await provider.GetSynchronizedObjectId(conferenceId, participantId);
            if (!await ParticipantHasSubscribed(conferenceId, participantId, syncObjId)) return;

            if (_cachedSyncObjectNotifications.TryGetValue(syncObjId, out var notification))
            {
                _cachedSyncObjectNotifications[syncObjId] = notification with
                {
                    ParticipantIds = notification.ParticipantIds.Add(participantId),
                };
                return;
            }

            var newValue = await provider.FetchValue(conferenceId, participantId);
            var previousValue = await _synchronizedObjectRepository.Create(conferenceId, syncObjId, newValue);

            _cachedSyncObjectNotifications[syncObjId] = new SynchronizedObjectUpdatedNotification(conferenceId,
                new List<string> {participantId}.ToImmutableList(), syncObjId, newValue, previousValue);
        }

        private async ValueTask<bool> ParticipantHasSubscribed(string conferenceId, string participantId,
            string syncObjId)
        {
            var subscriptions = await _subscriptionsRepository.Get(conferenceId, participantId);
            return subscriptions?.Contains(syncObjId) == true;
        }
    }
}
