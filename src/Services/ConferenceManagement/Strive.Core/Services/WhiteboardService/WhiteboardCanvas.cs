using System;
using System.Collections.Generic;

namespace Strive.Core.Services.WhiteboardService
{
    public record WhiteboardCanvas(IReadOnlyList<StoredCanvasObject> Objects, string BackgroundColor, double PanX,
        double PanY)
    {
        public static WhiteboardCanvas Empty => new(Array.Empty<StoredCanvasObject>(), "white", 0, 0);
    }
}
