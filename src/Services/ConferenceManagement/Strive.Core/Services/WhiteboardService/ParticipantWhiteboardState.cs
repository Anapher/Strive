using System.Collections.Immutable;

namespace Strive.Core.Services.WhiteboardService
{
    public record ParticipantWhiteboardState(IImmutableList<VersionedAction> UndoList,
        IImmutableList<VersionedAction> RedoList);
}
