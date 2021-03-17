using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Core.Services.Rooms.Notifications;
using PaderConference.Messaging.SFU.Dto;

namespace PaderConference.Messaging.SFU.NotificationHandlers
{
    public class ParticipantsRoomChangedNotificationHandler : INotificationHandler<ParticipantsRoomChangedNotification>
    {
        private readonly ISfuConferenceInfoProvider _provider;
        private readonly ISfuNotifier _notifier;
        private readonly IRoomRepository _roomRepository;

        public ParticipantsRoomChangedNotificationHandler(ISfuConferenceInfoProvider provider, ISfuNotifier notifier,
            IRoomRepository roomRepository)
        {
            _provider = provider;
            _notifier = notifier;
            _roomRepository = roomRepository;
        }

        public async Task Handle(ParticipantsRoomChangedNotification notification, CancellationToken cancellationToken)
        {
            var synchronizedRooms = await _roomRepository.GetParticipantRooms(notification.ConferenceId);

            var updatedParticipants = FilterParticipants(synchronizedRooms, notification.Participants);
            var participantsLeft = notification.Participants.Where(x => !synchronizedRooms.ContainsKey(x.Id))
                .Select(x => x.Id).ToList();

            var update = new SfuConferenceInfoUpdate(updatedParticipants,
                ImmutableDictionary<string, SfuParticipantPermissions>.Empty, participantsLeft);

            await _notifier.Update(notification.ConferenceId, update);
        }

        private IReadOnlyDictionary<string, string> FilterParticipants(
            IReadOnlyDictionary<string, string> participantMap, IReadOnlyList<Participant> participants)
        {
            return participantMap.Where(x => participants.Any(participant => x.Key == participant.Id))
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
