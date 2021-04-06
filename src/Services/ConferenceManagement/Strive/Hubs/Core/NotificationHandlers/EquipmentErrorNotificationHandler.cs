using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Strive.Core.Services.Equipment.Notifications;
using Strive.Hubs.Core.Responses;

namespace Strive.Hubs.Core.NotificationHandlers
{
    public class EquipmentErrorNotificationHandler : INotificationHandler<EquipmentErrorNotification>
    {
        private readonly IHubContext<CoreHub> _hubContext;

        public EquipmentErrorNotificationHandler(IHubContext<CoreHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(EquipmentErrorNotification notification, CancellationToken cancellationToken)
        {
            var (participant, connectionId, error) = notification;

            var dto = new EquipmentErrorDto(connectionId, error);

            var group = _hubContext.Clients.Group(CoreHubGroups.OfParticipant(participant));
            await group.EquipmentError(dto, cancellationToken);
        }
    }
}
