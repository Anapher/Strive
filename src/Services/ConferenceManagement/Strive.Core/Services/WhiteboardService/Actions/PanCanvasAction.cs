namespace Strive.Core.Services.WhiteboardService.Actions
{
    public record PanCanvasAction (double PanX, double PanY, string ParticipantId) : CanvasAction(ParticipantId)
    {
        public override WhiteboardCanvasUpdate? Execute(WhiteboardCanvas canvas, ICanvasActionUtils utils,
            int nextVersion)
        {
            var undoAction = new PanCanvasAction(canvas.PanX, canvas.PanY, ParticipantId);
            var updatedWhiteboard = canvas with {PanX = PanX, PanY = PanY};

            return new WhiteboardCanvasUpdate(updatedWhiteboard, undoAction);
        }
    }
}
