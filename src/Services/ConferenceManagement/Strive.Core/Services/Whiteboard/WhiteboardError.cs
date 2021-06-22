using Strive.Core.Dto;
using Strive.Core.Errors;

namespace Strive.Core.Services.Whiteboard
{
    public class WhiteboardError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error WhiteboardNotFound =>
            NotFound("The whiteboard was not found.", ServiceErrorCode.Whiteboard_NotFound);

        public static Error UndoNotAvailable =>
            NotFound("Undo is not available as no action was executed.", ServiceErrorCode.Whiteboard_UndoNotAvailable);

        public static Error RedoNotAvailable =>
            NotFound("Redo is not available as no action was undone.", ServiceErrorCode.Whiteboard_RedoNotAvailable);
    }
}
