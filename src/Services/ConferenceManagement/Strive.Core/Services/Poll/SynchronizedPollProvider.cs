using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Poll.Types.SingleChoice;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Poll
{
    public class SynchronizedPollProvider : SynchronizedObjectProvider<SynchronizedPoll>
    {
        private const string PROP_POLL_ID = "pollId";

        private readonly IPollRepository _repository;
        private readonly IMediator _mediator;

        public SynchronizedPollProvider(IPollRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public override string Id => SynchronizedObjectIds.POLL;

        public override async ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            var polls = await _mediator.Send(new FetchParticipantPollsRequest(participant));
            return polls.Select(x => SynchronizedPoll.SyncObjId(x.Id));
        }

        protected override async ValueTask<SynchronizedPoll> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            var pollId = synchronizedObjectId.Parameters[PROP_POLL_ID];

            var poll = await _repository.GetPoll(conferenceId, pollId);
            if (poll == null)
                return GetDefaultSynchronizedPoll(pollId);

            var state = await _repository.GetPollState(conferenceId, pollId) ?? PollState.Default;
            return new SynchronizedPoll(pollId, poll.Instruction, poll.Config, state, poll.CreatedOn);
        }

        public static SynchronizedObjectId BuildSyncObjId(string pollId)
        {
            return new(SynchronizedObjectIds.POLL, new Dictionary<string, string> {{PROP_POLL_ID, pollId}});
        }

        private static SynchronizedPoll GetDefaultSynchronizedPoll(string pollId)
        {
            return new(pollId, new SingleChoiceInstruction(new[] {""}), new PollConfig("", false, false), new PollState(
                false, false), DateTimeOffset.MinValue);
        }
    }
}
