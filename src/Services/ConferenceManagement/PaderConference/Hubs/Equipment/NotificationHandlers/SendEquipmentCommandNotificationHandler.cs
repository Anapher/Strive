using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Services.Equipment.Notifications;
using PaderConference.Hubs.Equipment.Responses;

namespace PaderConference.Hubs.Equipment.NotificationHandlers
{
    public class SendEquipmentCommandNotificationHandler : INotificationHandler<SendEquipmentCommandNotification>
    {
        private readonly IHubContext<EquipmentHub> _hubContext;

        public SendEquipmentCommandNotificationHandler(IHubContext<EquipmentHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(SendEquipmentCommandNotification notification, CancellationToken cancellationToken)
        {
            var command = new EquipmentCommandDto(notification.Source, notification.DeviceId, notification.Action);
            await _hubContext.Clients.Client(notification.ConnectionId)
                .SendEquipmentCommand(command, cancellationToken);
        }
    }
}
