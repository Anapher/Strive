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
            var joinedParticipant = new Participant(conferenceId, participantId);

            var permissions = await _permissionRepository.GetPermissions(joinedParticipant);

            return new SynchronizedParticipantPermissions(permissions);
        }

        public override ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            return new(GetObjIdOfParticipant(participant.Id).Yield());
        }

        public static SynchronizedObjectId GetObjIdOfParticipant(string participantId)
        {
            return new(SynchronizedObjectIds.PARTICIPANT_PERMISSIONS,
                new Dictionary<string, string> {{PROP_PARTICIPANT_ID, participantId}});
        }
    }
}
