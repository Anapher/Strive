using System.Threading.Tasks;
using PaderConference.Core.NewServices.Permissions.Gateways;

namespace PaderConference.Core.NewServices.Permissions
{
    public class ParticipantPermissions : IParticipantPermissions
    {
        private readonly IAggregatedPermissionRepository _permissionsRepo;

        public ParticipantPermissions(IAggregatedPermissionRepository permissionsRepo)
        {
            _permissionsRepo = permissionsRepo;
        }

        public async ValueTask<IPermissionStack> FetchForParticipant(string conferenceId, string participantId)
        {
            return new RepositoryPermissionStack(_permissionsRepo, conferenceId, participantId);
        }
    }
}
