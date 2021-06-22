using System.Collections.Immutable;

namespace Strive.Core.Services.Whiteboard
{
    public record ParticipantWhiteboardState(IImmutableList<VersionedAction> UndoList,
        IImmutableList<VersionedAction> RedoList);
}
