using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Poll
{
    public record SynchronizedPoll(string Id, PollInstruction Instruction, PollConfig Config, PollState State)
    {
        public static SynchronizedObjectId SyncObjId(string pollId)
        {
            return SynchronizedPollProvider.BuildSyncObjId(pollId);
        }
    }
}
