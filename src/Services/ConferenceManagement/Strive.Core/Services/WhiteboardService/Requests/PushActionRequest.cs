using MediatR;
using Strive.Core.Services.WhiteboardService.Actions;

namespace Strive.Core.Services.WhiteboardService.Requests
{
    public record PushActionRequest
        (string ConferenceId, string RoomId, string WhiteboardId, CanvasAction Action) : IRequest;
}
