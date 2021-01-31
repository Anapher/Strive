using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.NewServices.Permissions.Gateways
{
    public interface ITemporaryPermissionRepository
    {
        ValueTask SetTemporaryPermission(string conferenceId, string participantId, string key, JValue value);

        ValueTask RemoveTemporaryPermission(string conferenceId, string participantId, string key);

        ValueTask<Dictionary<string, JValue>> FetchTemporaryPermissions(string conferenceId, string participantId);
    }
}
