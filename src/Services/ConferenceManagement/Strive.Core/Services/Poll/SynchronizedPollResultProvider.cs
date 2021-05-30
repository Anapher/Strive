using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Poll
{
    public class SynchronizedPollResultProvider : SynchronizedObjectProvider<SynchronizedPollResult>
    {
        private const string PROP_POLL_ID = "pollId";

        private readonly IPollRepository _repository;
        private readonly IMediator _mediator;
        private readonly IParticipantPermissions _participantPermissions;

        public SynchronizedPollResultProvider(IPollRepository repository, IMediator mediator,
            IParticipantPermissions participantPermissions)
        {
            _repository = repository;
            _mediator = mediator;
            _participantPermissions = participantPermissions;
        }

        public override string Id => SynchronizedObjectIds.POLL_RESULT;

        public override async ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            var polls = await _mediator.Send(new FetchParticipantPollsRequest(participant));
            var pollStates = await _repository.GetStateOfAllPolls(participant.ConferenceId);

            var mappedPolls = polls.Select(poll =>
                (poll, state: pollStates.ContainsKey(poll.Id) ? pollStates[poll.Id] : null)).ToList();

            if (ContainsUnpublishedPoll(mappedPolls) && !await CanParticipantViewUnpublishedPolls(participant))
            {
                mappedPolls = mappedPolls.Where(x => x.state?.ResultsPublished == true).ToList();
            }

            return mappedPolls.Select(x => SynchronizedPollResult.SyncObjId(x.poll.Id));
        }

        protected override async ValueTask<SynchronizedPollResult> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            var pollId = synchronizedObjectId.Parameters[PROP_POLL_ID];

            var result = await _mediator.Send(new FetchPollResultsRequest(conferenceId, pollId));
            return new SynchronizedPollResult(pollId, result.Results, result.ParticipantTokenToId);
        }

        public static SynchronizedObjectId BuildSyncObjId(string pollId)
        {
            return new(SynchronizedObjectIds.POLL, new Dictionary<string, string> {{PROP_POLL_ID, pollId}});
        }

        private bool ContainsUnpublishedPoll(IEnumerable<(Poll, PollState? state)> polls)
        {
            return polls.Any(x => x.state?.ResultsPublished != true);
        }

        private async ValueTask<bool> CanParticipantViewUnpublishedPolls(Participant participant)
        {
            var permissions = await _participantPermissions.FetchForParticipant(participant);
            return await permissions.GetPermissionValue(DefinedPermissions.Poll.CanSeeUnpublishedPollResults);
        }
    }
}
