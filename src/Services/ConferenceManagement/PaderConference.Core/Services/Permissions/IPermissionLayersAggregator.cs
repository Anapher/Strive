using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.Services.Permissions
{
    public interface IPermissionLayersAggregator
    {
        ValueTask<Dictionary<string, JValue>> FetchAggregatedPermissions(Participant participant);

        ValueTask<List<PermissionLayer>> FetchParticipantPermissionLayers(Participant participant);
    }
}
