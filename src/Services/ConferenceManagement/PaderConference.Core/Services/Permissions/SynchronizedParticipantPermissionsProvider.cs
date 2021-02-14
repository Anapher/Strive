using System.Threading.Tasks;
using PaderConference.Core.Services.Permissions.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Permissions
{
    public class
        SynchronizedParticipantPermissionsProvider : SynchronizedObjectProvider<SynchronizedParticipantPermissions>
    {
        private readonly IAggregatedPermissionRepository _permissionRepository;

        public SynchronizedParticipantPermissionsProvider(IAggregatedPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public override ValueTask<bool> CanSubscribe(string conferenceId, string participantId)
        {
            return new(true);
        }

        protected override async ValueTask<SynchronizedParticipantPermissions> InternalFetchValue(string conferenceId,
            string participantId)
        {
            var permissions = await _permissionRepository.GetPermissions(conferenceId, participantId);
            return new SynchronizedParticipantPermissions(permissions);
        }

        public override ValueTask<string> GetSynchronizedObjectId(string conferenceId, string participantId)
        {
            return new(SynchronizedObjectIds.ParticipantPermissions(participantId));
        }
    }
}
