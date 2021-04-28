using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes.Providers.TalkingStick.Gateways;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick
{
    public interface ITalkingStickModeHandler
    {
        ValueTask InvalidateTalkingSceneWithLockAcquired(string conferenceId, string roomId);
    }

    public class TalkingStickModeHandler : ITalkingStickModeHandler
    {
        private readonly IMediator _mediator;
        private readonly ITalkingStickRepository _repository;

        public TalkingStickModeHandler(IMediator mediator, ITalkingStickRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async ValueTask InvalidateTalkingSceneWithLockAcquired(string conferenceId, string roomId)
        {
            var scenes =
                await _mediator.FetchSynchronizedObject<SynchronizedScene>(conferenceId,
                    SynchronizedScene.SyncObjId(roomId));

            var talkingStickScene = GetTalkingStickScene(scenes);
            if (talkingStickScene == null) return;

            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(conferenceId,
                SynchronizedRooms.SyncObjId);

            var currentSpeaker = await _repository.GetCurrentSpeaker(conferenceId, roomId);
            currentSpeaker = await VerifyCurrentSpeakerInRoom(currentSpeaker, roomId, rooms);

            if (currentSpeaker == null)
            {
                await ElectNewCurrentSpeaker(conferenceId, roomId, rooms, talkingStickScene.Mode);
            }
        }

        private TalkingStickScene? GetTalkingStickScene(SynchronizedScene scenes)
        {
            return scenes.SceneStack.OfType<TalkingStickScene>().LastOrDefault();
        }

        private async ValueTask<Participant?> VerifyCurrentSpeakerInRoom(Participant? currentSpeaker, string roomId,
            SynchronizedRooms rooms)
        {
            if (currentSpeaker == null) return null;

            if (CheckParticipantIsInRoom(currentSpeaker.Value, roomId, rooms))
                return currentSpeaker;

            await _repository.RemoveCurrentSpeaker(currentSpeaker.Value.ConferenceId, roomId);
            return null;
        }

        private async ValueTask ElectNewCurrentSpeaker(string conferenceId, string roomId, SynchronizedRooms rooms,
            TalkingStickMode mode)
        {
            if (mode == TalkingStickMode.Queue)
            {
                var nextSpeaker = await _repository.Dequeue(conferenceId, roomId);
                if (nextSpeaker == null) return;

                if (!CheckParticipantIsInRoom(nextSpeaker.Value, roomId, rooms))
                {
                    await ElectNewCurrentSpeaker(conferenceId, roomId, rooms, mode);
                    return;
                }

                await _repository.SetCurrentSpeakerAndRemoveFromQueue(nextSpeaker.Value, roomId);
            }
        }

        private static bool CheckParticipantIsInRoom(Participant participant, string roomId, SynchronizedRooms rooms)
        {
            return rooms.Participants.TryGetValue(participant.Id, out var actualRoomId) && actualRoomId == roomId;
        }
    }
}
