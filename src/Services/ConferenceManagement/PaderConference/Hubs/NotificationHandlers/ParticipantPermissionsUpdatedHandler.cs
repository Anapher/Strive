using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Services.Permissions.Notifications;

namespace PaderConference.Hubs.NotificationHandlers
{
    public class ParticipantPermissionsUpdatedHandler : INotificationHandler<ParticipantPermissionsUpdatedNotification>
    {
        private readonly IHubContext<CoreHub> _hubContext;

        public ParticipantPermissionsUpdatedHandler(IHubContext<CoreHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(ParticipantPermissionsUpdatedNotification notification,
            CancellationToken cancellationToken)
        {
            foreach (var (participantId, permissions) in notification.UpdatedPermissions)
            {
                await _hubContext.Clients.Client(participantId).OnPermissionsUpdated(permissions, cancellationToken);
            }
        }
    }
}
