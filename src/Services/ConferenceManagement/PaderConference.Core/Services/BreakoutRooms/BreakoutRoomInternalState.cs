using System.Collections.Generic;

namespace PaderConference.Core.Services.BreakoutRooms
{
    public record BreakoutRoomInternalState(BreakoutRoomsConfig Config, IReadOnlyList<string> OpenedRooms,
        string? TimerTokenId);
}
