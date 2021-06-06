using System.Collections.Generic;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Poll
{
    public record SynchronizedPollResult(string PollId, PollResults Results, int ParticipantsAnswered,
        IReadOnlyDictionary<string, string>? TokenIdToParticipant) : SanitizedPollResult(Results, ParticipantsAnswered,
        TokenIdToParticipant)
    {
        public static SynchronizedObjectId SyncObjId(string pollId)
        {
            return SynchronizedPollResultProvider.BuildSyncObjId(pollId);
        }
    }
}
