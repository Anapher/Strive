using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Strive.Core.Interfaces.Gateways.Repositories;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.Rooms.Requests;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Rooms.UseCases
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
            var (conferenceId, assignments) = request;

            var changedRooms = new Dictionary<Participant, ParticipantRoomChangeInfo>();
            foreach (var (participantId, roomId) in assignments)
            {
                var participant = new Participant(conferenceId, participantId);

                _logger.LogDebug("Switch participant {participant} to room {roomId}", participant, roomId);
                string? previousRoomId;
                try
                {
                    previousRoomId = await _roomRepository.SetParticipantRoom(participant, roomId);
                }
                catch (ConcurrencyException)
                {
                    _logger.LogWarning("Concurrency exception occurred, continue");
                    continue;
                }

                changedRooms[participant] = ParticipantRoomChangeInfo.Switched(previousRoomId, roomId);
            }

            if (changedRooms.Any())
            {
                await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId, SynchronizedRooms.SyncObjId));
                await _mediator.Publish(new ParticipantsRoomChangedNotification(conferenceId, changedRooms));
            }

            return Unit.Value;
        }
    }
}
