using System;

namespace PaderConference.Core.Services.BreakoutRoom
{
    public class ActiveBreakoutRoomState
    {
        public ActiveBreakoutRoomState(int amount, DateTimeOffset? deadline, string? description)
        {
            Amount = amount;
            Deadline = deadline;
            Description = description;
        }

        public int Amount { get; }

        public DateTimeOffset? Deadline { get; }

        public string? Description { get; }
    }
}
