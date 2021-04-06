using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Strive.Core.Extensions;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Scenes
{
    public class SynchronizedSceneProvider : SynchronizedObjectProvider<SynchronizedScene>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ISceneRepository _sceneRepository;
        private readonly IMediator _mediator;
        private readonly SceneOptions _options;

        public SynchronizedSceneProvider(IRoomRepository roomRepository, ISceneRepository sceneRepository,
            IMediator mediator, IOptions<SceneOptions> options)
        {
            _roomRepository = roomRepository;
            _sceneRepository = sceneRepository;
            _mediator = mediator;
            _options = options.Value;
        }

        public override string Id { get; } = SynchronizedObjectIds.SCENE;

        public override async ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            var roomId = await _roomRepository.GetRoomOfParticipant(participant);
            if (roomId == null)
                return Enumerable.Empty<SynchronizedObjectId>();

            return SynchronizedScene.SyncObjId(roomId).Yield();
        }

        protected override async ValueTask<SynchronizedScene> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            var roomId = synchronizedObjectId.Parameters[SynchronizedScene.PROP_ROOMID];
            var scene = await _sceneRepository.GetScene(conferenceId, roomId);
            scene ??= GetDefaultScene(roomId);

            var availableScenes = await _mediator.Send(new FetchAvailableScenesRequest(conferenceId, roomId));

            return new SynchronizedScene(scene, availableScenes);
        }

        private ActiveScene GetDefaultScene(string roomId)
        {
            if (roomId == RoomOptions.DEFAULT_ROOM_ID)
                return _options.DefaultRoomState;

            return _options.RoomState;
        }
    }
}
