using System;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Poll
{
    public record SynchronizedPoll(string Id, PollInstruction Instruction, PollConfig Config, PollState State,
        DateTimeOffset CreatedOn)
    {
        public static SynchronizedObjectId SyncObjId(string pollId)
        {
            return SynchronizedPollProvider.BuildSyncObjId(pollId);
        }
    }
}
