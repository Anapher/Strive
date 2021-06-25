using Strive.Core.Services.WhiteboardService.Actions;

namespace Strive.Core.Services.WhiteboardService.PushActions
{
    public record PanCanvasPushAction(double PanX, double PanY) : CanvasPushAction
    {
        public override CanvasAction ConvertToAction(string participantId)
        {
            return new PanCanvasAction(PanX, PanY, participantId);
        }
    }
}
