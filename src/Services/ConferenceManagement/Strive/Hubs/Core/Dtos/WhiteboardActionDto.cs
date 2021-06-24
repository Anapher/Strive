using System.Collections.Generic;
using Strive.Core.Services.WhiteboardService.Actions;
using Strive.Core.Services.WhiteboardService.CanvasData;

namespace Strive.Hubs.Core.Dtos
{
    public abstract record WhiteboardActionDto(string WhiteboardId);

    public record WhiteboardActionAddDto(string WhiteboardId, CanvasObject Object) : WhiteboardActionDto(WhiteboardId);

    public record WhiteboardActionDeleteDto
        (string WhiteboardId, IReadOnlyList<string> ObjectIds) : WhiteboardActionDto(WhiteboardId);

    public record WhiteboardActionUpdateDto
        (string WhiteboardId, IReadOnlyList<CanvasObjectPatch> Patches) : WhiteboardActionDto(WhiteboardId);

    public record WhiteboardActionPanDto
        (string WhiteboardId, double PanX, double PanY) : WhiteboardActionDto(WhiteboardId);
}
