using System;

namespace PaderConference.Core.Services.BreakoutRooms
{
    public record BreakoutRoomsConfig(int Amount, TimeSpan? Duration, string? Description);
}
