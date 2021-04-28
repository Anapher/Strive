using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes.Providers.TalkingStick.Gateways;
using Strive.Core.Services.Scenes.Providers.TalkingStick.Requests;
using Strive.Core.Services.Synchronization.Extensions;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick.UseCases
{
    public class TalkingStickReturnUseCase : IRequestHandler<TalkingStickReturnRequest>
    {
        private readonly IMediator _mediator;
        private readonly ITalkingStickRepository _repository;
        private readonly ITalkingStickModeHandler _modeHandler;

        public TalkingStickReturnUseCase(IMediator mediator, ITalkingStickRepository repository,
            ITalkingStickModeHandler modeHandler)
        {
            _mediator = mediator;
            _repository = repository;
            _modeHandler = modeHandler;
        }

        public async Task<Unit> Handle(TalkingStickReturnRequest request, CancellationToken cancellationToken)
        {
            var participant = request.Participant;

            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(participant.ConferenceId,
                SynchronizedRooms.SyncObjId);

            if (!rooms.Participants.TryGetValue(participant.Id, out var roomId))
                throw SceneError.RoomNotFound.ToException();

            await using (await _repository.LockRoom(participant.ConferenceId, roomId))
            {
                var currentSpeaker = await _repository.GetCurrentSpeaker(participant.ConferenceId, roomId);
                if (currentSpeaker != null && currentSpeaker.Value.Equals(participant))
                {
                    await _repository.RemoveCurrentSpeaker(currentSpeaker.Value.ConferenceId, roomId);
                    await _modeHandler.InvalidateTalkingSceneWithLockAcquired(currentSpeaker.Value.ConferenceId,
                        roomId);
                }
                else
                {
                    return Unit.Value;
                }
            }

            await _mediator.Send(new UpdateSynchronizedObjectRequest(participant.ConferenceId,
                SynchronizedSceneTalkingStick.SyncObjId(roomId)));

            return Unit.Value;
        }
    }
}
