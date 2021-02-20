using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.Permissions.Gateways;

namespace PaderConference.Core.Services.Permissions.NotificationHandlers
{
    public class ParticipantLeftNotificationHandler : INotificationHandler<ParticipantLeftNotification>
    {
        private readonly IAggregatedPermissionRepository _permissionRepository;
        private readonly ITemporaryPermissionRepository _temporaryPermissionRepository;

        public ParticipantLeftNotificationHandler(IAggregatedPermissionRepository permissionRepository,
            ITemporaryPermissionRepository temporaryPermissionRepository)
        {
            _permissionRepository = permissionRepository;
            _temporaryPermissionRepository = temporaryPermissionRepository;
        }

        public async Task Handle(ParticipantLeftNotification notification, CancellationToken cancellationToken)
        {
            var (participant, _) = notification;

            await _permissionRepository.DeletePermissions(participant);
            await _temporaryPermissionRepository.RemoveAllTemporaryPermissions(participant);
        }
    }
}
