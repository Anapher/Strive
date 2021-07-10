using System.Collections.Generic;
using Strive.Core.Services.WhiteboardService.Actions;

namespace Strive.Core.Services.WhiteboardService.PushActions
{
    public record DeleteCanvasPushAction(IReadOnlyList<string> ObjectIds) : CanvasPushAction
    {
        public override CanvasAction ConvertToAction(string participantId)
        {
            return new DeleteCanvasAction(ObjectIds, participantId);
        }
    }
}
