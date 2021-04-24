using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.Scenes.Providers.TalkingStick.Gateways;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick.NotificationHandlers
{
    public class RoomsRemovedNotificationHandler : INotificationHandler<RoomsRemovedNotification>
    {
        private readonly ITalkingStickRepository _repository;

        public RoomsRemovedNotificationHandler(ITalkingStickRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(RoomsRemovedNotification notification, CancellationToken cancellationToken)
        {
            var (conferenceId, removedRoomIds) = notification;

            foreach (var removedRoomId in removedRoomIds)
            {
                await _repository.RemoveCurrentSpeaker(conferenceId, removedRoomId);
                await _repository.ClearQueue(conferenceId, removedRoomId);
            }
        }
    }
}
