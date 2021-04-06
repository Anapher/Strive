using System.Collections.Generic;
using Strive.Core.Services.Synchronization;
using PermissionsDict = System.Collections.Generic.Dictionary<string, Newtonsoft.Json.Linq.JValue>;

namespace Strive.Core.Services.Permissions
{
    public record SynchronizedParticipantPermissions(PermissionsDict Permissions)
    {
        public const string PROP_PARTICIPANT_ID = "participantId";

        public static SynchronizedObjectId SyncObjId(string participantId)
        {
            return new(SynchronizedObjectIds.PARTICIPANT_PERMISSIONS,
                new Dictionary<string, string> {{PROP_PARTICIPANT_ID, participantId}});
        }
    }
}
