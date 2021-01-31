using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.NewServices.Permissions.Notifications;
using PaderConference.Hubs;

namespace PaderConference.SignalrNotificationHandler
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
            foreach (var permission in notification.UpdatedPermissions)
            {
                await _hubContext.Clients.Client(permission.Key)
                    .SendAsync(CoreHubMessages.Response.OnPermissionsUpdated, permission, cancellationToken);
            }
        }
    }
}
