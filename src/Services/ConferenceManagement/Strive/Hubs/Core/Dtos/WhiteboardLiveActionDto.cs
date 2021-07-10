using Strive.Core.Services.WhiteboardService;

namespace Strive.Hubs.Core.Dtos
{
    public record WhiteboardLiveActionDto(string WhiteboardId, CanvasLiveAction Action);
}
