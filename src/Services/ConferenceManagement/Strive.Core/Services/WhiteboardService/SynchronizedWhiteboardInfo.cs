using System.Collections.Generic;

namespace Strive.Core.Services.WhiteboardService
{
    public record SynchronizedWhiteboardInfo(string FriendlyName, bool AnyoneCanEdit, WhiteboardCanvas Canvas,
        IReadOnlyDictionary<string, SynchronizedParticipantState> ParticipantStates, int Version);
}
