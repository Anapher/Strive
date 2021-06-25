using Strive.Core.Services.WhiteboardService.PushActions;

namespace Strive.Hubs.Core.Dtos
{
    public record WhiteboardPushActionDto(string WhiteboardId, CanvasPushAction Action);
}
