using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Strive.Core.Extensions;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.Synchronization.Gateways;
using Strive.Core.Services.Synchronization.Notifications;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Synchronization.UseCases
{
    public class UpdateSubscriptionsUseCase : IRequestHandler<UpdateSubscriptionsRequest>
    {
        private readonly ISynchronizedObjectSubscriptionsRepository _subscriptionsRepository;
        private readonly IJoinedParticipantsRepository _joinedParticipantsRepository;
        private readonly IEnumerable<ISynchronizedObjectProvider> _providers;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateSubscriptionsUseCase> _logger;

        public UpdateSubscriptionsUseCase(ISynchronizedObjectSubscriptionsRepository subscriptionsRepository,
            IJoinedParticipantsRepository joinedParticipantsRepository,
            IEnumerable<ISynchronizedObjectProvider> providers, IMediator mediator,
            ILogger<UpdateSubscriptionsUseCase> logger)
        {
            _subscriptionsRepository = subscriptionsRepository;
            _joinedParticipantsRepository = joinedParticipantsRepository;
            _providers = providers;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateSubscriptionsRequest request, CancellationToken cancellationToken)
        {
            var participant = request.Participant;

            _logger.LogDebug("Update subscriptions for {participant}", participant);

            var subscriptions = new HashSet<SynchronizedObjectId>();
            foreach (var provider in _providers)
            {
                var availableSubscriptions = await provider.GetAvailableObjects(participant);
                subscriptions.UnionWith(availableSubscriptions);
            }

            _logger.LogDebug("Add {count} subscriptions", subscriptions.Count);

            var oldSubscriptions =
                await _subscriptionsRepository.GetSet(participant, subscriptions.Select(x => x.ToString()).ToList()) ??
                ImmutableList<string>.Empty;

            if (!await _joinedParticipantsRepository.IsParticipantJoined(participant))
            {
                _logger.LogWarning("The participant does not seem to be joined, remove all subscriptions");
                await _subscriptionsRepository.Remove(participant);
                return Unit.Value;
            }

            var added = subscriptions.Where(x => !oldSubscriptions.Contains(x.ToString())).ToList();
            await SendCurrentSynchronizedObjectValues(participant, added);

            var removed = oldSubscriptions.Except(subscriptions.Select(x => x.ToString()))
                .Select(SynchronizedObjectId.Parse).ToList();

            if (removed.Any() || added.Any())
                await _mediator.Publish(new ParticipantSubscriptionsUpdatedNotification(participant, removed, added));

            return Unit.Value;
        }

        private async ValueTask SendCurrentSynchronizedObjectValues(Participant participant,
            IEnumerable<SynchronizedObjectId> subscriptions)
        {
            var syncSubscriptionsObjId = SynchronizedSubscriptions.SyncObjId(participant.Id).ToString();

            foreach (var syncObjId in subscriptions)
            {
                _logger.LogDebug("Send current value of {syncObId} to participant.", syncObjId);

                // subscriptions are updated anyways after this handler completed
                if (syncObjId.ToString() == syncSubscriptionsObjId)
                    continue;

                await SendCurrentSynchronizedObjectValue(participant, syncObjId);
            }
        }

        private async ValueTask SendCurrentSynchronizedObjectValue(Participant participant,
            SynchronizedObjectId syncObjId)
        {
            var value = await _mediator.Send(new FetchSynchronizedObjectRequest(participant.ConferenceId, syncObjId));

            await _mediator.Publish(new SynchronizedObjectUpdatedNotification(participant.Yield().ToImmutableList(),
                syncObjId.ToString(), value, null));
        }
    }
}
