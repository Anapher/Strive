using System;

namespace Strive.Core.Services.BreakoutRooms
{
    public record BreakoutRoomsConfig(int Amount, DateTimeOffset? Deadline, string? Description);
}
