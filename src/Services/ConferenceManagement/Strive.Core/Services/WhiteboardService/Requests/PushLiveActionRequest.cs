using MediatR;

namespace Strive.Core.Services.WhiteboardService.Requests
{
    public record PushLiveActionRequest(string ConferenceId, string RoomId, string ParticipantId, string WhiteboardId,
        CanvasLiveAction Action) : IRequest;
}
