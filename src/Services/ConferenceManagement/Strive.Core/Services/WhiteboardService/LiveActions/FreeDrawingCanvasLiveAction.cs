using System.Collections.Generic;

namespace Strive.Core.Services.WhiteboardService.LiveActions
{
    public record FreeDrawingCanvasLiveAction
        (string Color, double Width, IReadOnlyList<CanvasPoint> AppendPoints) : CanvasLiveAction;
}
