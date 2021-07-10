namespace Strive.Core.Services.WhiteboardService
{
    public abstract record CanvasPushAction
    {
        public abstract CanvasAction ConvertToAction(string participantId);
    }
}
