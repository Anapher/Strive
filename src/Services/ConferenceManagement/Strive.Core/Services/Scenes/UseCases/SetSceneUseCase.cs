using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Core.Services.Scenes.Requests;
using Strive.Core.Services.Synchronization.Extensions;
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

            await using (await _sceneRepository.LockScene(conferenceId, roomId))
            {
                var previousScene = await _sceneRepository.GetScene(conferenceId, roomId);
                previousScene ??= SynchronizedSceneProvider.GetDefaultActiveScene();

                await _sceneRepository.SetScene(conferenceId, roomId, previousScene with {SelectedScene = scene});
            }

            // for optimistic concurrency
            await _mediator.Send(new ClearCacheRequest());

            if (!await IsRoomExisting(conferenceId, roomId))
            {
                await _sceneRepository.RemoveScene(conferenceId, roomId);
                throw SceneError.RoomNotFound.ToException();
            }

            await _mediator.Send(new UpdateScenesRequest(conferenceId, roomId));
            return Unit.Value;
        }

        private async ValueTask<bool> IsRoomExisting(string conferenceId, string roomId)
        {
            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(conferenceId,
                SynchronizedRooms.SyncObjId);
            return rooms.Rooms.Any(x => x.RoomId == roomId);
        }
    }
}
