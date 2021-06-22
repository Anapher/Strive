namespace Strive.Core.Services.Whiteboard.Actions
{
    public abstract record CanvasAction(string ParticipantId)
    {
        public abstract WhiteboardCanvasUpdate Execute(WhiteboardCanvas canvas, ICanvasActionUtils utils);
    }
}
