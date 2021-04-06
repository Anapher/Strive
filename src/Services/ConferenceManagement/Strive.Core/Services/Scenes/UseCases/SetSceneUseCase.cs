using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Scenes.UseCases
{
    public class SetSceneUseCase : IRequestHandler<SetSceneRequest>
    {
        private readonly ISceneRepository _sceneRepository;
        private readonly IMediator _mediator;

        public SetSceneUseCase(ISceneRepository sceneRepository, IMediator mediator)
        {
            _sceneRepository = sceneRepository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(SetSceneRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomId, scene) = request;

            var previousScene = await _sceneRepository.GetScene(conferenceId, roomId);
            await _sceneRepository.SetScene(conferenceId, roomId, scene);

            if (!await IsRoomExisting(conferenceId, roomId))
            {
                await _sceneRepository.RemoveScene(conferenceId, roomId);
                throw SceneError.RoomNotFound.ToException();
            }

            if (scene.Scene != null && !await IsSceneAvailable(conferenceId, roomId, scene.Scene))
            {
                if (previousScene != null)
                    await _sceneRepository.SetScene(conferenceId, roomId, previousScene);
                else await _sceneRepository.RemoveScene(conferenceId, roomId);

                throw SceneError.InvalidScene.ToException();
            }

            await _mediator.Send(
                new UpdateSynchronizedObjectRequest(conferenceId, SynchronizedScene.SyncObjId(roomId)));

            return Unit.Value;
        }

        private async ValueTask<bool> IsRoomExisting(string conferenceId, string roomId)
        {
            var rooms = (SynchronizedRooms?) await _mediator.Send(
                new FetchSynchronizedObjectRequest(conferenceId, SynchronizedRooms.SyncObjId));
            return rooms?.Rooms.Any(x => x.RoomId == roomId) == true;
        }

        private async ValueTask<bool> IsSceneAvailable(string conferenceId, string roomId, IScene scene)
        {
            var availableScenes = await _mediator.Send(new FetchAvailableScenesRequest(conferenceId, roomId));
            return availableScenes.Contains(scene);
        }
    }
}
