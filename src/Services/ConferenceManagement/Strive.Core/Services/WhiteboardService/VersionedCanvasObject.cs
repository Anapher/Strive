using Strive.Core.Services.WhiteboardService.CanvasData;

namespace Strive.Core.Services.WhiteboardService
{
    public record VersionedCanvasObject(CanvasObject Data, string Id, int Version) : StoredCanvasObject(Data, Id);
}
