using System;

namespace PaderConference.Core.Services.BreakoutRooms
{
    public record BreakoutRoomsConfig(int Amount, DateTimeOffset? Deadline, string? Description);
}
