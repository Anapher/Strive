using System.Collections.Immutable;

namespace Strive.Core.Services.WhiteboardService
{
    public record Whiteboard (string Id, string FriendlyName, bool AnyoneCanEdit, WhiteboardCanvas Canvas,
        IImmutableDictionary<string, ParticipantWhiteboardState> ParticipantStates, int Version);
}
