using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Synchronization
{
    public class SynchronizedSubscriptionsProvider : SynchronizedObjectProvider<SynchronizedSubscriptions>
    {
        private readonly IMediator _mediator;

        public SynchronizedSubscriptionsProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override string Id { get; } = SynchronizedObjectIds.SUBSCRIPTIONS;

        public override ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            return new(SynchronizedSubscriptions.SyncObjId(participant.Id).Yield());
        }

        protected override async ValueTask<SynchronizedSubscriptions> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            var participantId = synchronizedObjectId.Parameters[SynchronizedSubscriptions.PROP_PARTICIPANT_ID];
            var participant = new Participant(conferenceId, participantId);

            var subscriptions = await _mediator.Send(new FetchParticipantSubscriptionsRequest(participant));
            return new SynchronizedSubscriptions(subscriptions.ToDictionary(x => x.ToString(), _ => true));
        }
    }
}
