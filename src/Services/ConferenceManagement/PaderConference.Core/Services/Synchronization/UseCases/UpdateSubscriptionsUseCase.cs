using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Synchronization.UseCases
{
    public class UpdateSubscriptionsUseCase : IRequestHandler<UpdateSubscriptionsRequest>
    {
        private readonly ISynchronizedObjectSubscriptionsRepository _subscriptionsRepository;
        private readonly ISynchronizedObjectRepository _synchronizedObjectRepository;
        private readonly IJoinedParticipantsRepository _joinedParticipantsRepository;
        private readonly IEnumerable<ISynchronizedObjectProvider> _providers;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateSubscriptionsUseCase> _logger;

        public UpdateSubscriptionsUseCase(ISynchronizedObjectSubscriptionsRepository subscriptionsRepository,
            ISynchronizedObjectRepository synchronizedObjectRepository,
            IJoinedParticipantsRepository joinedParticipantsRepository,
            IEnumerable<ISynchronizedObjectProvider> providers, IMediator mediator,
            ILogger<UpdateSubscriptionsUseCase> logger)
        {
            _subscriptionsRepository = subscriptionsRepository;
            _synchronizedObjectRepository = synchronizedObjectRepository;
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

            var newSubscriptions = subscriptions.Where(x => !oldSubscriptions.Contains(x.ToString()));
            await SendCurrentSynchronizedObjectValues(participant, newSubscriptions);

            var removedSubscriptions = oldSubscriptions.Except(subscriptions.Select(x => x.ToString())).ToList();
            if (removedSubscriptions.Any())
                await _mediator.Publish(
                    new ParticipantSubscriptionsRemovedNotification(participant, removedSubscriptions));

            return Unit.Value;
        }

        private async ValueTask SendCurrentSynchronizedObjectValues(Participant participant,
            IEnumerable<SynchronizedObjectId> subscriptions)
        {
            foreach (var syncObjId in subscriptions)
            {
                _logger.LogDebug("Send current value of {syncObId} to participant.", syncObjId);

                var value = await GetCurrentValueOfSynchronizedObject(participant.ConferenceId, syncObjId);

                await _mediator.Publish(new SynchronizedObjectUpdatedNotification(participant.Yield().ToImmutableList(),
                    syncObjId.ToString(), value, null));
            }
        }

        private async Task<object> GetCurrentValueOfSynchronizedObject(string conferenceId,
            SynchronizedObjectId syncObjId)
        {
            var provider = _providers.First(x => x.Id == syncObjId.Id);

            var currentStoredValue =
                await _synchronizedObjectRepository.Get(conferenceId, syncObjId.ToString(), provider.Type);

            if (currentStoredValue != null) return currentStoredValue;

            var currentValue = await provider.FetchValue(conferenceId, syncObjId);
            await _synchronizedObjectRepository.Create(conferenceId, syncObjId.ToString(), currentValue, provider.Type);

            return currentValue;
        }
    }
}
