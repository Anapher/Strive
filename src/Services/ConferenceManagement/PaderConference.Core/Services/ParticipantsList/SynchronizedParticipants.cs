using System.Collections.Generic;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.ParticipantsList
{
    public record SynchronizedParticipants(IReadOnlyDictionary<string, ParticipantData> Participants)
    {
        public static SynchronizedObjectId SyncObjId { get; } = new(SynchronizedObjectIds.PARTICIPANTS);
    }
}
