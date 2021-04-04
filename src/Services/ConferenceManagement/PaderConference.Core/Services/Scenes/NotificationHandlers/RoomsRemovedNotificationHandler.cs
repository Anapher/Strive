using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Rooms.Notifications;
using PaderConference.Core.Services.Scenes.Gateways;

namespace PaderConference.Core.Services.Scenes.NotificationHandlers
{
    public class RoomsRemovedNotificationHandler : INotificationHandler<RoomsRemovedNotification>
    {
        private readonly ISceneRepository _repository;

        public RoomsRemovedNotificationHandler(ISceneRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(RoomsRemovedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var removedRoomId in notification.RemovedRoomIds)
            {
                await _repository.RemoveScene(notification.ConferenceId, removedRoomId);
                await _repository.RemoveAvailableScenes(notification.ConferenceId, removedRoomId);
            }
        }
    }
}
