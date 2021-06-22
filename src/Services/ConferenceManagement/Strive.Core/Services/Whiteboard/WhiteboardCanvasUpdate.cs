using Strive.Core.Services.Whiteboard.Actions;

namespace Strive.Core.Services.Whiteboard
{
    public record WhiteboardCanvasUpdate(WhiteboardCanvas Canvas, CanvasAction UndoAction);
}
