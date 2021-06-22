using Microsoft.AspNetCore.JsonPatch;
using Strive.Core.Services.Whiteboard.CanvasData;

namespace Strive.Core.Services.Whiteboard.Actions
{
    public interface ICanvasActionUtils
    {
        JsonPatchDocument<CanvasObject> CreatePatch(CanvasObject original, CanvasObject modified);
    }
}
