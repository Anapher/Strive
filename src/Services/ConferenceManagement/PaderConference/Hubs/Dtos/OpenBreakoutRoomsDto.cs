using System;

namespace PaderConference.Hubs.Dtos
{
    public record OpenBreakoutRoomsDto(int Amount, DateTimeOffset? Deadline, string? Description,
        string[][]? AssignedRooms);
}
