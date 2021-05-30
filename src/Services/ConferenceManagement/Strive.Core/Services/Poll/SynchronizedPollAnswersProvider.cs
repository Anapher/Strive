using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Extensions;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Poll
{
    public class SynchronizedPollAnswersProvider : SynchronizedObjectProvider<SynchronizedPollAnswers>
    {
        private const string PROP_PARTICIPANT_ID = "participantId";

        private readonly IPollRepository _repository;

        public SynchronizedPollAnswersProvider(IPollRepository repository)
        {
            _repository = repository;
        }

        public override string Id => SynchronizedObjectIds.POLL_ANSWERS;

        public override async ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            return SynchronizedPollAnswers.SyncObjId(participant.Id).Yield();
        }

        protected override async ValueTask<SynchronizedPollAnswers> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            var participantId = synchronizedObjectId.Parameters[PROP_PARTICIPANT_ID];

            var answers = await _repository.GetPollAnswersOfParticipant(new Participant(conferenceId, participantId));
            return new SynchronizedPollAnswers(answers.ToDictionary(x => x.Key, x => x.Value));
        }

        public static SynchronizedObjectId BuildSyncObjId(string participantId)
        {
            return new(SynchronizedObjectIds.POLL, new Dictionary<string, string>
                {{PROP_PARTICIPANT_ID, participantId}});
        }
    }
}
