using System.Collections.Generic;

namespace Strive.Core.Services.Synchronization
{
    public record SynchronizedSubscriptions(IReadOnlyDictionary<string, bool> Subscriptions)
    {
        public const string PROP_PARTICIPANT_ID = "participantId";

        public static SynchronizedObjectId SyncObjId(string participantId)
        {
            return new(SynchronizedObjectIds.SUBSCRIPTIONS,
                new Dictionary<string, string> {{PROP_PARTICIPANT_ID, participantId}});
        }
    }
}
