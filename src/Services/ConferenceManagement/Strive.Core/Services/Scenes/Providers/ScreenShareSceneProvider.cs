using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Services.Media;
using Strive.Core.Services.Media.Dtos;
using Strive.Core.Services.Media.Gateways;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Core.Services.Scenes.Scenes;

namespace Strive.Core.Services.Scenes.Providers
{
    public class ScreenShareSceneProvider : ISceneProvider
    {
        private readonly IMediaStateRepository _mediaStateRepository;
        private readonly IRoomRepository _roomRepository;

        public ScreenShareSceneProvider(IMediaStateRepository mediaStateRepository, IRoomRepository roomRepository)
        {
            _mediaStateRepository = mediaStateRepository;
            _roomRepository = roomRepository;
        }

        public async ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId)
        {
            var participantsInRoom = await _roomRepository.GetParticipantsOfRoom(conferenceId, roomId);
            var mediaState = await _mediaStateRepository.Get(conferenceId);

            return participantsInRoom
                .Where(x => mediaState.TryGetValue(x.Id, out var streams) &&
                            streams.Producers.ContainsKey(ProducerSource.Screen))
                .Select(x => new ScreenShareScene(x.Id));
        }

        public async ValueTask<SceneUpdate> UpdateAvailableScenes(string conferenceId, string roomId,
            object synchronizedObject)
        {
            if (synchronizedObject is SynchronizedRooms || synchronizedObject is SynchronizedMediaState)
            {
                var scenes = await GetAvailableScenes(conferenceId, roomId);
                return SceneUpdate.UpdateRequired(scenes);
            }

            return SceneUpdate.NoUpdateRequired;
        }

        public bool IsProvided(IScene scene)
        {
            return scene is ScreenShareScene;
        }
    }
}
