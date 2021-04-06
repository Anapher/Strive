using System.Collections.Generic;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.ParticipantsList
{
    public record SynchronizedParticipants(IReadOnlyDictionary<string, ParticipantData> Participants)
    {
        public static SynchronizedObjectId SyncObjId { get; } = new(SynchronizedObjectIds.PARTICIPANTS);
    }
}
