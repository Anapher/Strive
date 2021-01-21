using System;

namespace PaderConference.Core.Services.BreakoutRoom.Dto
{
    public class BreakoutRoomsOptions
    {
        public TimeSpan? Duration { get; set; }

        public string? Description { get; set; }

        public int Amount { get; set; }
    }
}
