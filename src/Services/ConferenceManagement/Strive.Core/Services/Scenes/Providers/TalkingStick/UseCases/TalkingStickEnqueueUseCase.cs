using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Interfaces.Gateways.Repositories;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes.Providers.TalkingStick.Gateways;
using Strive.Core.Services.Scenes.Providers.TalkingStick.Requests;
using Strive.Core.Services.Synchronization.Extensions;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick.UseCases
{
    public class TalkingStickEnqueueUseCase : IRequestHandler<TalkingStickEnqueueRequest>
    {
        private readonly IMediator _mediator;
        private readonly ITalkingStickRepository _repository;
        private readonly ITalkingStickModeHandler _modeHandler;

        public TalkingStickEnqueueUseCase(IMediator mediator, ITalkingStickRepository repository,
            ITalkingStickModeHandler modeHandler)
        {
            _mediator = mediator;
            _repository = repository;
            _modeHandler = modeHandler;
        }

        public async Task<Unit> Handle(TalkingStickEnqueueRequest request, CancellationToken cancellationToken)
        {
            var (participant, remove) = request;

            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(participant.ConferenceId,
                SynchronizedRooms.SyncObjId);

            if (!rooms.Participants.TryGetValue(participant.Id, out var roomId))
                throw SceneError.RoomNotFound.ToException();

            await using (await _repository.LockRoom(participant.ConferenceId, roomId))
            {
                await _mediator.Send(new ClearCacheRequest());

                if (!await CheckParticipantIsInRoom(participant, roomId))
                    throw new ConcurrencyException("Participant switched room.");

                if (remove)
                {
                    await _repository.RemoveFromQueue(participant.Yield(), roomId);
                }
                else
                {
                    var queue = await _repository.FetchQueue(participant.ConferenceId, roomId);
                    if (!queue.Contains(participant.Id))
                        await _repository.Enqueue(participant, roomId);

                    await _modeHandler.InvalidateTalkingSceneWithLockAcquired(participant.ConferenceId, roomId);
                }
            }

            await _mediator.Send(new UpdateSynchronizedObjectRequest(participant.ConferenceId,
                SynchronizedSceneTalkingStick.SyncObjId(roomId)));

            return Unit.Value;
        }

        private async Task<bool> CheckParticipantIsInRoom(Participant participant, string roomId)
        {
            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(participant.ConferenceId,
                SynchronizedRooms.SyncObjId);

            return rooms.Participants.TryGetValue(participant.Id, out var actualRoomId) && actualRoomId == roomId;
        }
    }
}
