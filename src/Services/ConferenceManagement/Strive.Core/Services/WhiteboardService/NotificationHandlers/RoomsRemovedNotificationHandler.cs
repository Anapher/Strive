using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.WhiteboardService.Gateways;

namespace Strive.Core.Services.WhiteboardService.NotificationHandlers
{
    public class RoomsRemovedNotificationHandler : INotificationHandler<RoomsRemovedNotification>
    {
        private readonly IWhiteboardRepository _repository;

        public RoomsRemovedNotificationHandler(IWhiteboardRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(RoomsRemovedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var removedRoomId in notification.RemovedRoomIds)
            {
                await _repository.DeleteAllOfRoom(notification.ConferenceId, removedRoomId);
            }
        }
    }
}
