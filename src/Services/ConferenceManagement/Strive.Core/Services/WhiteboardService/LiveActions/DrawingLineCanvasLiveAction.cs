namespace Strive.Core.Services.WhiteboardService.LiveActions
{
    public record DrawingLineCanvasLiveAction (string Color, double StrokeWidth, double StartX, double StartY,
        double EndX, double EndY) : CanvasLiveAction;
}
