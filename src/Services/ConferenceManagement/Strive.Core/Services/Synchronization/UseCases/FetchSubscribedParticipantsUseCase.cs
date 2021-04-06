using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Synchronization.Gateways;
using Strive.Core.Services.Synchronization.Requests;
using Strive.Core.Services.Synchronization.Utilities;

namespace Strive.Core.Services.Synchronization.UseCases
{
    public class
        FetchSubscribedParticipantsUseCase : IRequestHandler<FetchSubscribedParticipantsRequest,
            IReadOnlyList<Participant>>
    {
        private readonly ISynchronizedObjectSubscriptionsRepository _subscriptionsRepository;

        public FetchSubscribedParticipantsUseCase(ISynchronizedObjectSubscriptionsRepository subscriptionsRepository)
        {
            _subscriptionsRepository = subscriptionsRepository;
        }

        public async Task<IReadOnlyList<Participant>> Handle(FetchSubscribedParticipantsRequest request,
            CancellationToken cancellationToken)
        {
            var (conferenceId, synchronizedObjectId) = request;

            var subscriptions = await _subscriptionsRepository.GetOfConference(conferenceId);
            return ConferenceSubscriptionHelper.GetParticipantIdsSubscribedTo(subscriptions, synchronizedObjectId)
                .Select(id => new Participant(conferenceId, id)).ToList();
        }
    }
}
