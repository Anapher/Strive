using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.BreakoutRooms.Gateways;
using PaderConference.Core.Services.BreakoutRooms.Notifications;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Requests;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.BreakoutRooms.Internal
{
    public class ApplyBreakoutRoomUseCase : IRequestHandler<ApplyBreakoutRoomRequest, BreakoutRoomInternalState?>
    {
        private readonly IBreakoutRoomRepository _repository;
        private readonly IMediator _mediator;
        private readonly IScheduledMediator _scheduledMediator;
        private readonly BreakoutRoomsOptions _options;

        public ApplyBreakoutRoomUseCase(IBreakoutRoomRepository repository, IMediator mediator,
            IScheduledMediator scheduledMediator, IOptions<BreakoutRoomsOptions> options)
        {
            _repository = repository;
            _mediator = mediator;
            _scheduledMediator = scheduledMediator;
            _options = options.Value;
        }

        public async Task<BreakoutRoomInternalState?> Handle(ApplyBreakoutRoomRequest request,
            CancellationToken cancellationToken)
        {
            var (conferenceId, newState, @lock, createNew) = request;

            BreakoutRoomInternalState? internalState;
            await using (@lock ?? await _repository.LockBreakoutRooms(request.ConferenceId))
            {
                internalState = await ApplyState(conferenceId, newState, createNew);
            }

            await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId, SynchronizedBreakoutRooms.SyncObjId),
                cancellationToken);

            return internalState;
        }

        private async Task<BreakoutRoomInternalState?> ApplyState(string conferenceId, BreakoutRoomsConfig? newState,
            bool createNew)
        {
            var internalState = await _repository.Get(conferenceId);

            if (internalState != null && createNew)
                throw new ConcurrencyException("The breakout rooms are already open.");

            if (internalState == null && newState == null) return null;

            if (internalState?.TimerTokenId != null)
                await _scheduledMediator.Remove<BreakoutRoomTimerElapsedNotification>(internalState.TimerTokenId);

            if (newState == null)
            {
                await _mediator.Send(new RemoveRoomsRequest(conferenceId, internalState!.OpenedRooms));
                await _repository.Remove(conferenceId);
                return null;
            }

            var openedRooms = internalState?.OpenedRooms ?? ImmutableList<string>.Empty;
            openedRooms = await AdjustOpenedRooms(conferenceId, newState, openedRooms);
            var timerToken = await CreateTimer(newState.Deadline, conferenceId);

            internalState = new BreakoutRoomInternalState(newState, openedRooms, timerToken);
            await _repository.Set(conferenceId, internalState);

            return internalState;
        }

        private async Task<IReadOnlyList<string>> AdjustOpenedRooms(string conferenceId, BreakoutRoomsConfig state,
            IReadOnlyList<string> openedRooms)
        {
            if (state.Amount > openedRooms.Count)
            {
                var rooms = await GetRoomInformation(conferenceId, openedRooms);
                var roomsToCreate = FindMissingRoomsToCreate(state.Amount - openedRooms.Count, rooms);
                var createdRooms = await _mediator.Send(new CreateRoomsRequest(conferenceId, roomsToCreate));

                return openedRooms.Concat(createdRooms.Select(x => x.RoomId)).ToList();
            }

            if (state.Amount < openedRooms.Count)
                return await RemoveRooms(conferenceId, openedRooms.Count - state.Amount, openedRooms);

            return openedRooms;
        }

        private async Task<IReadOnlyList<Room>> GetRoomInformation(string conferenceId,
            IReadOnlyList<string> openedRooms)
        {
            if (!openedRooms.Any())
                return ImmutableList<Room>.Empty;

            var syncRooms = await FetchSynchronizedRooms(conferenceId);
            return MapOpenedRooms(syncRooms, openedRooms).ToList();
        }

        private async Task<SynchronizedRooms> FetchSynchronizedRooms(string conferenceId)
        {
            return (SynchronizedRooms) await _mediator.Send(
                new FetchSynchronizedObjectRequest(conferenceId, SynchronizedRooms.SyncObjId));
        }

        private IReadOnlyList<RoomCreationInfo> FindMissingRoomsToCreate(int amount, IReadOnlyList<Room> rooms)
        {
            var createRooms = new List<RoomCreationInfo>();

            var currentPos = 0;
            for (var i = 0; i < amount; i++)
            {
                // find first unused name
                string name;
                do
                {
                    name = _options.NamingStrategy.GetName(currentPos++);
                } while (rooms.Any(x => x.DisplayName == name));

                createRooms.Add(new RoomCreationInfo(name));
            }

            return createRooms;
        }

        private async Task<IReadOnlyList<string>> RemoveRooms(string conferenceId, int amount,
            IReadOnlyList<string> openedRooms)
        {
            var syncRooms = await FetchSynchronizedRooms(conferenceId);

            // order rooms by least participants, then by their naming index (desc)
            var removeRooms = MapOpenedRooms(syncRooms, openedRooms)
                .OrderBy(room => syncRooms.Participants.Count(participantMap => participantMap.Value == room.RoomId))
                .ThenByDescending(room => _options.NamingStrategy.ParseIndex(room.DisplayName)).Take(amount)
                .Select(x => x.RoomId).ToList();

            await _mediator.Send(new RemoveRoomsRequest(conferenceId, removeRooms));
            return openedRooms.Except(removeRooms).ToList();
        }

        private IEnumerable<Room> MapOpenedRooms(SynchronizedRooms syncRooms, IEnumerable<string> openedRooms)
        {
            return syncRooms.Rooms.Where(x => openedRooms.Contains(x.RoomId)).ToList();
        }

        private async Task<string?> CreateTimer(DateTimeOffset? deadline, string conferenceId)
        {
            if (deadline == null) return null;

            return await _scheduledMediator.Schedule(new BreakoutRoomTimerElapsedNotification(conferenceId),
                deadline.Value);
        }
    }
}
