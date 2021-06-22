using Strive.Core.Services.Whiteboard.Actions;

namespace Strive.Core.Services.Whiteboard
{
    public record VersionedAction(CanvasAction Action, int Version);
}
