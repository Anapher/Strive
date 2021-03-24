using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Services.ConferenceControl.Notifications;

namespace PaderConference.Hubs.Equipment.NotificationHandlers
{
    public class ParticipantLeftNotificationHandler : INotificationHandler<ParticipantLeftNotification>
    {
        private readonly IHubContext<EquipmentHub> _hubContext;

        public ParticipantLeftNotificationHandler(IHubContext<EquipmentHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(ParticipantLeftNotification notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.Group(EquipmentGroups.OfParticipant(notification.Participant))
                .OnRequestDisconnect(cancellationToken);
        }
    }
}
