using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Core.Services.Rooms.Requests;

namespace PaderConference.Core.Services.Rooms.NotificationHandlers
{
    public class ConferenceOpenedNotificationHandler : INotificationHandler<ConferenceOpenedNotification>
    {
        private readonly IJoinedParticipantsRepository _joinedParticipantsRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IMediator _mediator;
        private readonly RoomOptions _options;

        public ConferenceOpenedNotificationHandler(IJoinedParticipantsRepository joinedParticipantsRepository,
            IRoomRepository roomRepository, IMediator mediator, IOptions<RoomOptions> options)
        {
            _joinedParticipantsRepository = joinedParticipantsRepository;
            _roomRepository = roomRepository;
            _mediator = mediator;
            _options = options.Value;
        }

        public async Task Handle(ConferenceOpenedNotification notification, CancellationToken cancellationToken)
        {
            var conferenceId = notification.ConferenceId;

            await EnsureDefaultRoomCreated(conferenceId);

            var participants = await _joinedParticipantsRepository.GetParticipantsOfConference(conferenceId);
            foreach (var participant in participants)
            {
                await MoveParticipantToDefaultRoom(participant);
            }
        }

        private async Task EnsureDefaultRoomCreated(string conferenceId)
        {
            var defaultRoom = GetDefaultRoom();
            await _roomRepository.CreateRoom(conferenceId, defaultRoom);
        }

        private Room GetDefaultRoom()
        {
            return new(RoomOptions.DEFAULT_ROOM_ID, _options.DefaultRoomName);
        }

        private async Task MoveParticipantToDefaultRoom(Participant participant)
        {
            await _mediator.Send(new SetParticipantRoomRequest(participant, RoomOptions.DEFAULT_ROOM_ID));
        }
    }
}
