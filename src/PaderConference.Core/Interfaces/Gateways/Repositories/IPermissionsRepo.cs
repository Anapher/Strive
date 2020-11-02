using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public interface IPermissionsRepo
    {
        Task<T> GetPermissionsValue<T>(string participantId, string key);

        Task SetPermissions(string participantId, Dictionary<string, JsonElement> permissions);

        Task PublishPermissionsUpdated(string[] participantIds);
    }
}
