using System;

namespace PaderConference.Hubs.Core.Dtos
{
    public record OpenBreakoutRoomsDto(int Amount, DateTimeOffset? Deadline, string? Description,
        string[][]? AssignedRooms);
}
