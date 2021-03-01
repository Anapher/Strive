using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Synchronization.UseCases
{
    public class FetchParticipantSubscriptionsUseCase : IRequestHandler<FetchParticipantSubscriptionsRequest,
        IReadOnlyList<SynchronizedObjectId>>
    {
        private readonly ISynchronizedObjectSubscriptionsRepository _subscriptionsRepository;

        public FetchParticipantSubscriptionsUseCase(ISynchronizedObjectSubscriptionsRepository subscriptionsRepository)
        {
            _subscriptionsRepository = subscriptionsRepository;
        }

        public async Task<IReadOnlyList<SynchronizedObjectId>> Handle(FetchParticipantSubscriptionsRequest request,
            CancellationToken cancellationToken)
        {
            var syncObjects = await _subscriptionsRepository.Get(request.Participant);
            if (syncObjects == null)
                return ImmutableList<SynchronizedObjectId>.Empty;

            return syncObjects.Select(SynchronizedObjectId.Parse).ToList();
        }
    }
}
