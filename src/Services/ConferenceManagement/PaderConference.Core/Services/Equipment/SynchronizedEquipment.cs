using System.Collections.Generic;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Equipment
{
    public record SynchronizedEquipment(IReadOnlyDictionary<string, EquipmentConnection> Connections)
    {
        public const string PROP_PARTICIPANT_ID = "participantId";

        public static SynchronizedObjectId SyncObjId(string participantId)
        {
            return new(SynchronizedObjectIds.EQUIPMENT,
                new Dictionary<string, string> {{PROP_PARTICIPANT_ID, participantId}});
        }
    }
}
