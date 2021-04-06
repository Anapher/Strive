using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.BreakoutRooms;
using Strive.Core.Services.Scenes.Modes;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Scenes.Providers
{
    public class BreakoutRoomSceneProvider : ISceneProvider
    {
        private readonly IMediator _mediator;

        public BreakoutRoomSceneProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId)
        {
            var state = (SynchronizedBreakoutRooms?) await _mediator.Send(
                new FetchSynchronizedObjectRequest(conferenceId, SynchronizedBreakoutRooms.SyncObjId));

            if (state == null)
                return Enumerable.Empty<IScene>();

            return GetAvailableScenes(state);
        }

        public ValueTask<SceneUpdate> UpdateAvailableScenes(string conferenceId, string roomId,
            object synchronizedObject)
        {
            if (synchronizedObject is SynchronizedBreakoutRooms syncBreakoutRooms)
            {
                var scenes = GetAvailableScenes(syncBreakoutRooms);
                return new ValueTask<SceneUpdate>(SceneUpdate.UpdateRequired(scenes));
            }

            return new ValueTask<SceneUpdate>(SceneUpdate.NoUpdateRequired);
        }

        private static IEnumerable<IScene> GetAvailableScenes(SynchronizedBreakoutRooms syncBreakoutRooms)
        {
            if (syncBreakoutRooms.Active != null)
                yield return BreakoutRoomScene.Instance;
        }

        public bool IsProvided(IScene scene)
        {
            return scene is BreakoutRoomScene;
        }
    }
}
