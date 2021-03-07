using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Synchronization
{
    public class SynchronizedSubscriptionsProvider : SynchronizedObjectProvider<SynchronizedSubscriptions>
    {
        private const string PROP_PARTICIPANT_ID = "participantId";
        private readonly IMediator _mediator;

        public SynchronizedSubscriptionsProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override string Id { get; } = SynchronizedObjectIds.SUBSCRIPTIONS;

        public override ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            return new(GetObjIdOfParticipant(participant.Id).Yield());
        }

        protected override async ValueTask<SynchronizedSubscriptions> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            var participantId = synchronizedObjectId.Parameters[PROP_PARTICIPANT_ID];
            var participant = new Participant(conferenceId, participantId);

            var subscriptions = await _mediator.Send(new FetchParticipantSubscriptionsRequest(participant));
            return new SynchronizedSubscriptions(subscriptions.ToDictionary(x => x.ToString(), _ => true));
        }

        public static SynchronizedObjectId GetObjIdOfParticipant(string participantId)
        {
            return new(SynchronizedObjectIds.SUBSCRIPTIONS,
                new Dictionary<string, string> {{PROP_PARTICIPANT_ID, participantId}});
        }
    }
}
