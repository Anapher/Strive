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
    public class TalkingStickPassUseCase : IRequestHandler<TalkingStickPassRequest>
    {
        private readonly IMediator _mediator;
        private readonly ITalkingStickRepository _repository;
        private readonly ITalkingStickModeHandler _modeHandler;

        public TalkingStickPassUseCase(IMediator mediator, ITalkingStickRepository repository,
            ITalkingStickModeHandler modeHandler)
        {
            _mediator = mediator;
            _repository = repository;
            _modeHandler = modeHandler;
        }

        public async Task<Unit> Handle(TalkingStickPassRequest request, CancellationToken cancellationToken)
        {
            var (participant, roomId, failIfHasSpeaker) = request;

            await using (await _repository.LockRoom(participant.ConferenceId, roomId))
            {
                await _mediator.Send(new ClearCacheRequest());

                if (failIfHasSpeaker)
                {
                    var currentSpeaker = await _repository.GetCurrentSpeaker(participant.ConferenceId, roomId);
                    if (currentSpeaker != null)
                        throw SceneError.AlreadyHasSpeaker.ToException();
                }

                if (!await CheckParticipantIsInRoom(participant, roomId))
                    throw new ParticipantNotFoundException(participant);

                await _repository.SetCurrentSpeakerAndRemoveFromQueue(participant, roomId);
                await _modeHandler.InvalidateTalkingSceneWithLockAcquired(participant.ConferenceId, roomId);
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
