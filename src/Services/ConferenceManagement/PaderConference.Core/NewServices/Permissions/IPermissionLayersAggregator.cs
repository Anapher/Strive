using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.NewServices.Permissions
{
    public interface IPermissionLayersAggregator
    {
        ValueTask<Dictionary<string, JValue>> FetchAggregatedPermissions(string conferenceId, string participantId);

        ValueTask<List<PermissionLayer>> FetchParticipantPermissionLayers(string conferenceId, string participantId);
    }
}
