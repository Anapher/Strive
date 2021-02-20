using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.Services.Permissions.Gateways
{
    public interface IAggregatedPermissionRepository
    {
        ValueTask SetPermissions(Participant participant, Dictionary<string, JValue> permissions);

        ValueTask<T?> GetPermissionsValue<T>(Participant participant, string key);

        ValueTask<Dictionary<string, JValue>> GetPermissions(Participant participant);

        ValueTask DeletePermissions(Participant participant);
    }
}
