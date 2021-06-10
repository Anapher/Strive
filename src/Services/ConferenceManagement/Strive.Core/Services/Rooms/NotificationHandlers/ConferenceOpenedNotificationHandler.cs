using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.Rooms.Requests;

namespace Strive.Core.Services.Rooms.NotificationHandlers
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
            await MoveParticipantsToDefaultRoom(conferenceId, participants);
        }

        private async Task EnsureDefaultRoomCreated(string conferenceId)
        {
            var defaultRoom = GetDefaultRoom();
            await _roomRepository.CreateRoom(conferenceId, defaultRoom);

            await _mediator.Publish(new RoomsCreatedNotification(conferenceId, new[] {defaultRoom.RoomId}));
        }

        private Room GetDefaultRoom()
        {
            return new(RoomOptions.DEFAULT_ROOM_ID, _options.DefaultRoomName);
        }

        private async Task MoveParticipantsToDefaultRoom(string conferenceId, IEnumerable<Participant> participant)
        {
            await _mediator.Send(new SetParticipantRoomRequest(conferenceId,
                participant.Select(x => (x.Id, RoomOptions.DEFAULT_ROOM_ID))));
        }
    }
}
