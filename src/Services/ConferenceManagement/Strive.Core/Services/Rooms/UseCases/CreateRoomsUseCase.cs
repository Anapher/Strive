using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Strive.Core.Interfaces.Gateways.Repositories;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.Rooms.Requests;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Rooms.UseCases
{
    public class CreateRoomsUseCase : IRequestHandler<CreateRoomsRequest, IReadOnlyList<Room>>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IOpenConferenceRepository _openConferenceRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateRoomsUseCase> _logger;

        public CreateRoomsUseCase(IRoomRepository roomRepository, IOpenConferenceRepository openConferenceRepository,
            IMediator mediator, ILogger<CreateRoomsUseCase> logger)
        {
            _roomRepository = roomRepository;
            _openConferenceRepository = openConferenceRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<IReadOnlyList<Room>> Handle(CreateRoomsRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomCreationInfos) = request;
            var rooms = roomCreationInfos.Select(x => new Room(Guid.NewGuid().ToString("N"), x.DisplayName)).ToList();

            _logger.LogDebug("Create {count} rooms", rooms.Count);
            await CreateRooms(rooms, conferenceId);

            if (!await _openConferenceRepository.IsOpen(conferenceId))
            {
                _logger.LogWarning("The conference was not open, revert creating rooms");
                await RevertRoomCreations(rooms, conferenceId);

                throw new ConcurrencyException("The conference is not open.");
            }

            await _mediator.Send(
                new UpdateSynchronizedObjectRequest(request.ConferenceId, SynchronizedRooms.SyncObjId));

            await _mediator.Publish(new RoomsCreatedNotification(conferenceId, rooms.Select(x => x.RoomId).ToList()));
            return rooms;
        }

        private async Task CreateRooms(IEnumerable<Room> rooms, string conferenceId)
        {
            foreach (var roomToCreate in rooms)
            {
                // as we are using GUIDs as room ids, this cannot go wrong nor overwrite existing rooms
                await _roomRepository.CreateRoom(conferenceId, roomToCreate);
            }
        }

        private async Task RevertRoomCreations(IEnumerable<Room> createdRooms, string conferenceId)
        {
            // the conference is not open, revert everything
            // we can safely revert it by removing all rooms, as the GUIDs are unique
            // we can assume that all participant mappings are empty as these are concurrency checked
            foreach (var room in createdRooms)
            {
                await _roomRepository.RemoveRoom(conferenceId, room.RoomId);
            }
        }
    }
}
