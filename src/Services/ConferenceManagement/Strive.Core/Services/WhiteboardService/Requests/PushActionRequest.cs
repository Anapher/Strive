using MediatR;
using Strive.Core.Services.WhiteboardService.PushActions;

namespace Strive.Core.Services.WhiteboardService.Requests
{
    public record PushActionRequest (string ConferenceId, string RoomId, string WhiteboardId, string ParticipantId,
        CanvasPushAction Action) : IRequest;
}
