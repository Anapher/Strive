using System;
using MediatR;

namespace Strive.Core.Services.Whiteboard.Requests
{
    public record UpdateWhiteboardRequest(string ConferenceId, string RoomId, string WhiteboardId,
        Func<Whiteboard, Whiteboard> Action) : IRequest;
}
