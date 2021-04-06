using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Permissions
{
    public record SynchronizedTemporaryPermissions(
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, JValue>> Assigned)
    {
        public static readonly SynchronizedObjectId SyncObjId = new(SynchronizedObjectIds.TEMPORARY_PERMISSIONS);
    }
}
