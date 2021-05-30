using System.Collections.Generic;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Poll
{
    public record SynchronizedPollResult(string PollId, PollResults Results,
        IReadOnlyDictionary<string, string>? ParticipantTokenToId) : SanitizedPollResult(Results, ParticipantTokenToId)
    {
        public static SynchronizedObjectId SyncObjId(string pollId)
        {
            return SynchronizedPollResultProvider.BuildSyncObjId(pollId);
        }
    }
}
