using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Strive.Core.Interfaces.Gateways;

namespace Strive.Core.Services.Permissions.Gateways
{
    public interface ITemporaryPermissionRepository : IStateRepository
    {
        ValueTask SetTemporaryPermission(Participant participant, string key, JValue value);

        ValueTask RemoveTemporaryPermission(Participant participant, string key);

        ValueTask<IReadOnlyDictionary<string, JValue>> FetchTemporaryPermissions(Participant participant);

        ValueTask RemoveAllTemporaryPermissions(Participant participant);

        ValueTask<IReadOnlyDictionary<string, IReadOnlyDictionary<string, JValue>>> FetchConferenceTemporaryPermissions(
            string conferenceId);
    }
}
