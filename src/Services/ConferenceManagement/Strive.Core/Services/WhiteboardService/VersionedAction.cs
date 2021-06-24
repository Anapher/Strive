using Strive.Core.Services.WhiteboardService.Actions;

namespace Strive.Core.Services.WhiteboardService
{
    public record VersionedAction(CanvasAction Action, int Version);
}
