using System.Collections.Generic;
using Strive.Core.Services.WhiteboardService.Actions;

namespace Strive.Core.Services.WhiteboardService.LiveActions
{
    public record ModifyingCanvasLiveAction(IReadOnlyList<CanvasObjectPatch> Patches) : CanvasLiveAction;
}
