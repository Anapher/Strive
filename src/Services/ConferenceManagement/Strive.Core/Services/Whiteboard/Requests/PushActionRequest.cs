using MediatR;
using Strive.Core.Services.Whiteboard.Actions;

namespace Strive.Core.Services.Whiteboard.Requests
{
    public record PushActionRequest
        (string ConferenceId, string RoomId, string WhiteboardId, CanvasAction Action) : IRequest;
}
