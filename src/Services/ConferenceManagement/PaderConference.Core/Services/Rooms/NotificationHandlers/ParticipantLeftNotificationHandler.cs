using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.Rooms.Gateways;

namespace PaderConference.Core.Services.Rooms.NotificationHandlers
{
    public class ParticipantLeftNotificationHandler : INotificationHandler<ParticipantLeftNotification>
    {
        private readonly IRoomRepository _roomRepository;

        public ParticipantLeftNotificationHandler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task Handle(ParticipantLeftNotification notification, CancellationToken cancellationToken)
        {
            await _roomRepository.UnsetParticipantRoom(notification.ConferenceId, notification.ParticipantId);
        }
    }
}
