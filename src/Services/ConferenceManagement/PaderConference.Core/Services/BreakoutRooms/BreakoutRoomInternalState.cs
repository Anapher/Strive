using System.Collections.Generic;

namespace PaderConference.Core.Services.BreakoutRooms
{
    public record BreakoutRoomInternalState(ActiveBreakoutRoomState State, IReadOnlyList<string> OpenedRooms,
        string? TimerTokenId);
}
