using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Core.Services.Rooms.Notifications;

namespace Strive.Core.Services.Rooms.NotificationHandlers
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
            await _mediator.Publish(new ParticipantsRoomChangedNotification(participant.ConferenceId,
                participant.Yield().ToList(), true));
        }
    }
}
