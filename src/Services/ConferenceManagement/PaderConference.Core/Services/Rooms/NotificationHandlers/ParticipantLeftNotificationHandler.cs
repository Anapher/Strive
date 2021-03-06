using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Core.Services.Rooms.Notifications;

namespace PaderConference.Core.Services.Rooms.NotificationHandlers
{
    public class ParticipantLeftNotificationHandler : INotificationHandler<ParticipantLeftNotification>
    {
        private readonly IMediator _mediator;
        private readonly IRoomRepository _roomRepository;

        public ParticipantLeftNotificationHandler(IMediator mediator, IRoomRepository roomRepository)
        {
            _mediator = mediator;
            _roomRepository = roomRepository;
        }

        public async Task Handle(ParticipantLeftNotification notification, CancellationToken cancellationToken)
        {
            var (participant, _) = notification;

            await _roomRepository.UnsetParticipantRoom(participant);
            await _mediator.Publish(
                new ParticipantsRoomChangedNotification(participant.ConferenceId, participant.Yield(), true));
        }
    }
}
