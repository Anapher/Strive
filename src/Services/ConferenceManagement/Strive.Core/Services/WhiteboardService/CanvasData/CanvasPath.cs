#pragma warning disable 8618

using System.Collections.Generic;

namespace Strive.Core.Services.WhiteboardService.CanvasData
{
    public record CanvasPath : CanvasObject
    {
        public IReadOnlyList<CanvasPoint> Path { get; set; }
    }
}
