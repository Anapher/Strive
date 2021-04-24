using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Messaging.SFU.Dto;

namespace Strive.Messaging.SFU.NotificationHandlers
{
    public class ParticipantsRoomChangedNotificationHandler : INotificationHandler<ParticipantsRoomChangedNotification>
    {
        private readonly ISfuNotifier _notifier;
        private readonly IRoomRepository _roomRepository;

        public ParticipantsRoomChangedNotificationHandler(ISfuNotifier notifier, IRoomRepository roomRepository)
        {
            _notifier = notifier;
            _roomRepository = roomRepository;
        }

        public async Task Handle(ParticipantsRoomChangedNotification notification, CancellationToken cancellationToken)
        {
            var updatedParticipants = notification.Participants.Where(x => x.Value.TargetRoom != null)
                .ToDictionary(x => x.Key.Id, x => x.Value.TargetRoom!);

            var participantsLeft = notification.Participants.Where(x => x.Value.TargetRoom == null)
                .Select(x => x.Key.Id).ToList();

            var update = new SfuConferenceInfoUpdate(updatedParticipants,
                ImmutableDictionary<string, SfuParticipantPermissions>.Empty, participantsLeft);

            await _notifier.Update(notification.ConferenceId, update);
        }
    }
}
