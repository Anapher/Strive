using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.NewServices.Permissions
{
    public class PermissionLayersAggregator : IPermissionLayersAggregator
    {
        private readonly IEnumerable<IPermissionLayerProvider> _permissionProviders;

        public PermissionLayersAggregator(IEnumerable<IPermissionLayerProvider> permissionProviders)
        {
            _permissionProviders = permissionProviders;
        }

        public async ValueTask<Dictionary<string, JValue>> FetchAggregatedPermissions(string conferenceId,
            string participantId)
        {
            var layers = await FetchParticipantPermissionLayers(conferenceId, participantId);

            var stack = new CachedPermissionStack(layers.OrderBy(x => x.Order).Select(x => x.Permissions).ToList());
            return stack.Flatten();
        }

        public async ValueTask<List<PermissionLayer>> FetchParticipantPermissionLayers(string conferenceId,
            string participantId)
        {
            var layers = new List<PermissionLayer>();
            foreach (var provider in _permissionProviders)
            {
                layers.AddRange(await provider.FetchPermissionsForParticipant(conferenceId, participantId));
            }

            return layers;
        }
    }
}
