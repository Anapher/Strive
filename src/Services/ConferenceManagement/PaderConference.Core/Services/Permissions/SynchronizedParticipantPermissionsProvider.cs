using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Permissions.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Permissions
{
    public class
        SynchronizedParticipantPermissionsProvider : SynchronizedObjectProvider<SynchronizedParticipantPermissions>
    {
        private const string PROP_PARTICIPANT_ID = "participantId";

        private readonly IAggregatedPermissionRepository _permissionRepository;

        public SynchronizedParticipantPermissionsProvider(IAggregatedPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public override string Id { get; } = SynchronizedObjectIds.PARTICIPANT_PERMISSIONS;

        protected override async ValueTask<SynchronizedParticipantPermissions> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            var participantId = synchronizedObjectId.Parameters[PROP_PARTICIPANT_ID];
            var permissions = await _permissionRepository.GetPermissions(conferenceId, participantId);

            return new SynchronizedParticipantPermissions(permissions);
        }

        public override ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(string conferenceId,
            string participantId)
        {
            return new(new SynchronizedObjectId(Id,
                new Dictionary<string, string> {{PROP_PARTICIPANT_ID, participantId}}).Yield());
        }
    }
}
