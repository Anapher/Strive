using System.Collections.Generic;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Poll
{
    public record SynchronizedPollAnswers(IReadOnlyDictionary<string, PollAnswerWithKey> Answers)
    {
        public static SynchronizedObjectId SyncObjId(string participantId)
        {
            return SynchronizedPollAnswersProvider.BuildSyncObjId(participantId);
        }
    }
}
