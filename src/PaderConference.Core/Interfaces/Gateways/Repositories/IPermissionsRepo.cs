using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public interface IPermissionsRepo
    {
        Task<T> GetPermissionsValue<T>(string participantId, string key);

        Task SetPermissions(string participantId, Dictionary<string, JValue> permissions);

        Task PublishPermissionsUpdated(string[] participantIds);
    }
}
