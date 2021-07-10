namespace Strive.Core.Services.WhiteboardService
{
    public abstract record CanvasAction(string ParticipantId)
    {
        public abstract WhiteboardCanvasUpdate? Execute(WhiteboardCanvas canvas, ICanvasActionUtils utils,
            int nextVersion);
    }
}
