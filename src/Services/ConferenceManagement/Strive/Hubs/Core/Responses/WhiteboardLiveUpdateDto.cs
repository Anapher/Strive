using Strive.Core.Services.WhiteboardService;

namespace Strive.Hubs.Core.Responses
{
    public record WhiteboardLiveUpdateDto(string WhiteboardId, string ParticipantId, CanvasLiveAction Action);
}
