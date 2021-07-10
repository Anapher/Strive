using System;
using System.Linq;
using Strive.Core.Extensions;
using Strive.Core.Services.WhiteboardService.Actions;
using Strive.Core.Services.WhiteboardService.CanvasData;

namespace Strive.Core.Services.WhiteboardService.PushActions
{
    public record AddCanvasPushAction(CanvasObject Object) : CanvasPushAction
    {
        public override CanvasAction ConvertToAction(string participantId)
        {
            var objId = Guid.NewGuid().ToString("N");
            return new AddCanvasAction(
                Object.Yield().Select(x => new StoredCanvasObject(x, objId)).Select(x => new CanvasObjectRef(x, null))
                    .ToList(), participantId);
        }
    }
}
