using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.Services.Permissions.Gateways
{
    public interface ITemporaryPermissionRepository
    {
        ValueTask SetTemporaryPermission(Participant participant, string key, JValue value);

        ValueTask RemoveTemporaryPermission(Participant participant, string key);

        ValueTask<IReadOnlyDictionary<string, JValue>> FetchTemporaryPermissions(Participant participant);

        ValueTask RemoveAllTemporaryPermissions(Participant participant);
    }
}
