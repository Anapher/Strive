using System;

namespace PaderConference.Core.Services.BreakoutRooms
{
    public record ActiveBreakoutRoomState(int Amount, DateTimeOffset? Deadline, string? Description);
}
