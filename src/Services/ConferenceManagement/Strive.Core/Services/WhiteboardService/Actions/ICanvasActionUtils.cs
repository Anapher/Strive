using Microsoft.AspNetCore.JsonPatch;
using Strive.Core.Services.WhiteboardService.CanvasData;

namespace Strive.Core.Services.WhiteboardService.Actions
{
    public interface ICanvasActionUtils
    {
        JsonPatchDocument<CanvasObject> CreatePatch(CanvasObject original, CanvasObject modified);
    }
}
