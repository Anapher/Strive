using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Media;
using Strive.Core.Services.Media.Dtos;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Scenes.Utilities;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Scenes.Providers
{
    public class ScreenShareSceneProvider : ContentSceneProvider
    {
        private readonly IMediator _mediator;

        public ScreenShareSceneProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override bool IsProvided(IScene scene)
        {
            return scene is ScreenShareScene;
        }

        public override async ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId,
            IReadOnlyList<IScene> sceneStack)
        {
            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(conferenceId,
                SynchronizedRooms.SyncObjId);
            var mediaState =
                await _mediator.FetchSynchronizedObject<SynchronizedMediaState>(conferenceId,
                    SynchronizedMediaState.SyncObjId);

            var participantsInRoom = SceneUtilities.GetParticipantsOfRoom(rooms, roomId);

            return participantsInRoom
                .Where(participantId => mediaState.Streams.TryGetValue(participantId, out var streams) &&
                                        streams.Producers.ContainsKey(ProducerSource.Screen)).Select(participantId =>
                    new ScreenShareScene(participantId));
        }

        protected override async ValueTask<bool> InternalIsUpdateRequired(string conferenceId, string roomId,
            object synchronizedObject, object? previousValue)
        {
            if (synchronizedObject is SynchronizedRooms rooms)
            {
                if (SceneUtilities.ParticipantsOfRoomChanged(roomId, rooms, previousValue as SynchronizedRooms))
                    return true;
            }
            else if (synchronizedObject is SynchronizedMediaState)
            {
                return true;
            }

            return false;
        }
    }
}
