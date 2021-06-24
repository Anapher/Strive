namespace Strive.Core.Services.WhiteboardService.Actions
{
    public record CanvasActionPan (double PanX, double PanY, string ParticipantId) : CanvasAction(ParticipantId)
    {
        public override WhiteboardCanvasUpdate Execute(WhiteboardCanvas canvas, ICanvasActionUtils utils)
        {
            var undoAction = new CanvasActionPan(canvas.PanX, canvas.PanY, ParticipantId);
            var updatedWhiteboard = canvas with {PanX = PanX, PanY = PanY};

            return new WhiteboardCanvasUpdate(updatedWhiteboard, undoAction);
        }
    }
}
