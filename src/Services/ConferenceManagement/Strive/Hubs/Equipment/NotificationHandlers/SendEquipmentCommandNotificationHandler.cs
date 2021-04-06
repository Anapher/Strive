using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Strive.Core.Services.Equipment.Notifications;
using Strive.Hubs.Equipment.Responses;

namespace Strive.Hubs.Equipment.NotificationHandlers
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
