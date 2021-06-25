using Strive.Core.Services.WhiteboardService.Actions;

namespace Strive.Core.Services.WhiteboardService.PushActions
{
    public abstract record CanvasPushAction
    {
        public abstract CanvasAction ConvertToAction(string participantId);
    }
}
