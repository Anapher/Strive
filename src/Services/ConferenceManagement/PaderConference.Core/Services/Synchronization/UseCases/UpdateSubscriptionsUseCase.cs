using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
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
            IEnumerable<ISynchronizedObjectProvider> providers, IMediator mediator, ILogger logger)
        {
            _repository = repository;
            _providers = providers;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UpdateSubscriptionsRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, participantId) = request;

            var subscriptions = new Dictionary<string, ISynchronizedObjectProvider>();
            foreach (var provider in _providers)
            {
                if (await provider.CanSubscribe(conferenceId, participantId))
                {
                    var syncObjId = await provider.GetSynchronizedObjectId(conferenceId, participantId);
                    subscriptions.Add(syncObjId, provider);
                }
            }

            var oldSubscriptions = await _repository.GetSet(conferenceId, participantId, subscriptions.Keys.ToList()) ??
                                   ImmutableList<string>.Empty;

            var newSubscriptions = subscriptions.Keys.Except(oldSubscriptions);
            await SendCurrentSynchronizedObjectValues(conferenceId, participantId, newSubscriptions, subscriptions);

            var removedSubscriptions = oldSubscriptions.Except(subscriptions.Keys).ToList();
            if (removedSubscriptions.Any())
                await _mediator.Publish(
                    new ParticipantSubscriptionsRemovedNotification(conferenceId, participantId, removedSubscriptions));

            return Unit.Value;
        }

        private async ValueTask SendCurrentSynchronizedObjectValues(string conferenceId, string participantId,
            IEnumerable<string> subscriptions, IReadOnlyDictionary<string, ISynchronizedObjectProvider> providers)
        {
            foreach (var subscription in subscriptions)
            {
                var provider = providers[subscription];
                var value = await provider.FetchValue(conferenceId, participantId);

                await _mediator.Publish(new SynchronizedObjectUpdatedNotification(conferenceId,
                    new[] {participantId}.ToImmutableList(), subscription, value, null));
            }
        }
    }
}
