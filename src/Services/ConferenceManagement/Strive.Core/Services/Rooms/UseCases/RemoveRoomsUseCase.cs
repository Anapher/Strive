using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.Rooms.Requests;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Rooms.UseCases
{
    public class RemoveRoomsUseCase : IRequestHandler<RemoveRoomsRequest>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<RemoveRoomsUseCase> _logger;

        private readonly List<Participant> _participantsInRemovedRooms = new();

        public RemoveRoomsUseCase(IRoomRepository roomRepository, IMediator mediator,
            ILogger<RemoveRoomsUseCase> logger)
        {
            _roomRepository = roomRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveRoomsRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomIds) = request;

            var removedRooms = new List<string>();
            foreach (var roomId in roomIds)
            {
                if (roomId == RoomOptions.DEFAULT_ROOM_ID)
                {
                    _logger.LogWarning("Cannot remove the default room.");
                    continue;
                }

                var removed = await RemoveRoom(conferenceId, roomId);
                if (removed) removedRooms.Add(roomId);
            }

            if (removedRooms.Any())
            {
                await MoveParticipantsToDefaultRoom(conferenceId);

                await _mediator.Send(
                    new UpdateSynchronizedObjectRequest(request.ConferenceId, SynchronizedRooms.SyncObjId));

                await _mediator.Publish(new RoomsRemovedNotification(conferenceId, removedRooms));
            }

            return Unit.Value;
        }

        private async Task<bool> RemoveRoom(string conferenceId, string roomId)
        {
            var removed = await _roomRepository.RemoveRoom(conferenceId, roomId);
            if (removed)
            {
                // no possibility of a race condition, because after the deletion no new participants may join the room
                // and the room cannot be recreated as every new room gets a new GUID
                var participants = await _roomRepository.GetParticipantsOfRoom(conferenceId, roomId);
                _participantsInRemovedRooms.AddRange(participants);
            }
            else
            {
                _logger.LogDebug("The room {roomId} did not exist.", roomId);
            }

            return removed;
        }

        private async Task MoveParticipantsToDefaultRoom(string conferenceId)
        {
            var assignments = _participantsInRemovedRooms.Select(x => (x.Id, RoomOptions.DEFAULT_ROOM_ID));

            try
            {
                await _mediator.Send(new SetParticipantRoomRequest(conferenceId, assignments));
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "An error occurred on trying to switch participant {participant} to the default room");
            }
        }
    }
}
