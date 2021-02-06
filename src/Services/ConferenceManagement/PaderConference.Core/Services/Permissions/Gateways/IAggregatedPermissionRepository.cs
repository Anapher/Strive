using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.Services.Permissions.Gateways
{
    public interface IAggregatedPermissionRepository
    {
        ValueTask SetPermissions(string conferenceId, string participantId, Dictionary<string, JValue> permissions);

        ValueTask<T> GetPermissionsValue<T>(string conferenceId, string participantId, string key);
    }
}
