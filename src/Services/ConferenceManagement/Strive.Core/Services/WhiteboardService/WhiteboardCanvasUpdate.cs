using Strive.Core.Services.WhiteboardService.Actions;

namespace Strive.Core.Services.WhiteboardService
{
    public record WhiteboardCanvasUpdate(WhiteboardCanvas Canvas, CanvasAction UndoAction);
}
