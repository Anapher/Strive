using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Core.Services.Rooms.Notifications;
using PaderConference.Core.Services.Rooms.Requests;

namespace PaderConference.Core.Services.Rooms.UseCases
{
    public class SetParticipantRoomUseCase : IRequestHandler<SetParticipantRoomRequest>
    {
        private readonly IMediator _mediator;
        private readonly IRoomRepository _roomRepository;
        private readonly ILogger<SetParticipantRoomUseCase> _logger;

        public SetParticipantRoomUseCase(IMediator mediator, IRoomRepository roomRepository,
            ILogger<SetParticipantRoomUseCase> logger)
        {
            _mediator = mediator;
            _roomRepository = roomRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(SetParticipantRoomRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, participantId, roomId) = request;

            _logger.LogDebug("Switch participant {participantId} to room {roomId}", participantId, roomId);
            await _roomRepository.SetParticipantRoom(conferenceId, participantId, roomId);
            await _mediator.Publish(new ParticipantsRoomChangedNotification(conferenceId, new[] {participantId}));

            return Unit.Value;
        }
    }
}
