using System;

namespace PaderConference.Core.Services.BreakoutRoom
{
    public record ActiveBreakoutRoomState
    {
        /// <summary>
        ///     The current amount of breakout rooms that are open
        /// </summary>
        public int Amount { get; init; }

        /// <summary>
        ///     The deadline for work in breakout rooms
        /// </summary>
        public DateTimeOffset? Deadline { get; init; }

        /// <summary>
        ///     The current description (task)
        /// </summary>
        public string? Description { get; init; }
    }
}
