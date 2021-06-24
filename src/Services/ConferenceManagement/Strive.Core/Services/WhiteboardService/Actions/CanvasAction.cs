namespace Strive.Core.Services.WhiteboardService.Actions
{
    public abstract record CanvasAction(string ParticipantId)
    {
        public abstract WhiteboardCanvasUpdate? Execute(WhiteboardCanvas canvas, ICanvasActionUtils utils);
    }
}
