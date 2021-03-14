using System;
using MediatR;

namespace PaderConference.Core.Services.BreakoutRooms.Internal
{
    public record ApplyBreakoutRoomRequest (string ConferenceId, BreakoutRoomsConfig? State,
        IAsyncDisposable? AcquiredLock = null, bool CreateNew = false) : IRequest<BreakoutRoomInternalState?>;
}
