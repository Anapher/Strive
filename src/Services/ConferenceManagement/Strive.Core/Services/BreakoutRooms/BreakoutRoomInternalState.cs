using System.Collections.Generic;

namespace Strive.Core.Services.BreakoutRooms
{
    public record BreakoutRoomInternalState(BreakoutRoomsConfig Config, IReadOnlyList<string> OpenedRooms,
        string? TimerTokenId);
}
