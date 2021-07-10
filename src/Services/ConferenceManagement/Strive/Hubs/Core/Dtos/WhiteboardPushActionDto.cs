using Strive.Core.Services.WhiteboardService;

namespace Strive.Hubs.Core.Dtos
{
    public record WhiteboardPushActionDto(string WhiteboardId, CanvasPushAction Action);
}
