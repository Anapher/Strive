using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Synchronization.UseCases
{
    public class UpdateSubscriptionsUseCase : IRequestHandler<UpdateSubscriptionsRequest>
    {
        private readonly ISynchronizedObjectSubscriptionsRepository _repository;
        private readonly IEnumerable<ISynchronizedObjectProvider> _providers;
        private readonly IMediator _mediator;

        public UpdateSubscriptionsUseCase(ISynchronizedObjectSubscriptionsRepository repository,
            IEnumerable<ISynchronizedObjectProvider> providers, IMediator mediator)
        {
            _repository = repository;
            _providers = providers;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UpdateSubscriptionsRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, participantId) = request;

            var subscriptions = new List<SynchronizedObjectId>();
            foreach (var provider in _providers)
            {
                var availableSubscriptions = await provider.GetAvailableObjects(conferenceId, participantId);
                subscriptions.AddRange(availableSubscriptions);
            }

            var oldSubscriptions =
                await _repository.GetSet(conferenceId, participantId,
                    subscriptions.Select(x => x.ToString()).ToList()) ?? ImmutableList<string>.Empty;

            var newSubscriptions = subscriptions.Where(x => !oldSubscriptions.Contains(x.ToString()));
            await SendCurrentSynchronizedObjectValues(conferenceId, participantId, newSubscriptions);

            var removedSubscriptions = oldSubscriptions.Except(subscriptions.Select(x => x.ToString())).ToList();
            if (removedSubscriptions.Any())
                await _mediator.Publish(
                    new ParticipantSubscriptionsRemovedNotification(conferenceId, participantId, removedSubscriptions));

            return Unit.Value;
        }

        private async ValueTask SendCurrentSynchronizedObjectValues(string conferenceId, string participantId,
            IEnumerable<SynchronizedObjectId> subscriptions)
        {
            foreach (var syncObjId in subscriptions)
            {
                var provider = _providers.First(x => x.Id == syncObjId.Id);
                var value = await provider.FetchValue(conferenceId, syncObjId);

                await _mediator.Publish(new SynchronizedObjectUpdatedNotification(conferenceId,
                    new[] {participantId}.ToImmutableList(), syncObjId.ToString(), value, null));
            }
        }
    }
}
