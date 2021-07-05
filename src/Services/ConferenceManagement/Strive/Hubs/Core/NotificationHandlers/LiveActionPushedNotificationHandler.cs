using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Strive.Core.Services.WhiteboardService.Notifications;
using Strive.Hubs.Core.Responses;

namespace Strive.Hubs.Core.NotificationHandlers
{
    public class LiveActionPushedNotificationHandler : INotificationHandler<LiveActionPushedNotification>
    {
        private readonly IHubContext<CoreHub> _hubContext;

        public LiveActionPushedNotificationHandler(IHubContext<CoreHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(LiveActionPushedNotification notification, CancellationToken cancellationToken)
        {
            var connectionIds = notification.Participants.Select(CoreHubGroups.OfParticipant).ToList();

            await _hubContext.Clients.Groups(connectionIds).WhiteboardLiveUpdate(
                new WhiteboardLiveUpdateDto(notification.WhiteboardId, notification.SenderParticipantId,
                    notification.Action), cancellationToken);
        }
    }
}
