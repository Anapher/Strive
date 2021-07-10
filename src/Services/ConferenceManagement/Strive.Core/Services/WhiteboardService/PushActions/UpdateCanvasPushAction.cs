using System.Collections.Generic;
using Strive.Core.Services.WhiteboardService.Actions;

namespace Strive.Core.Services.WhiteboardService.PushActions
{
    public record UpdateCanvasPushAction(IReadOnlyList<CanvasObjectPatch> Patches) : CanvasPushAction
    {
        public override CanvasAction ConvertToAction(string participantId)
        {
            return new UpateCanvasAction(Patches, participantId);
        }
    }
}
