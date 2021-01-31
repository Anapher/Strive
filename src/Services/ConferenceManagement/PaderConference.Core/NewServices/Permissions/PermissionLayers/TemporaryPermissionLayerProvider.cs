using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.NewServices.Permissions.Gateways;

namespace PaderConference.Core.NewServices.Permissions.PermissionLayers
{
    public class TemporaryPermissionLayerProvider : IPermissionLayerProvider
    {
        private readonly ITemporaryPermissionRepository _temporaryPermissionRepository;

        public TemporaryPermissionLayerProvider(ITemporaryPermissionRepository temporaryPermissionRepository)
        {
            _temporaryPermissionRepository = temporaryPermissionRepository;
        }

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(string conferenceId,
            string participantId)
        {
            var permissions =
                await _temporaryPermissionRepository.FetchTemporaryPermissions(conferenceId, participantId);

            return new List<PermissionLayer> {CommonPermissionLayers.Temporary(permissions)};
        }
    }
}
