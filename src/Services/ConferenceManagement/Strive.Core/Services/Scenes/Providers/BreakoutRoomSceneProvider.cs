using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.BreakoutRooms;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Scenes.Providers
{
    public class BreakoutRoomSceneProvider : ContentSceneProvider
    {
        private readonly IMediator _mediator;

        public BreakoutRoomSceneProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override bool IsProvided(IScene scene)
        {
            return scene is BreakoutRoomScene;
        }

        public override async ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId,
            IReadOnlyList<IScene> sceneStack)
        {
            var state = await _mediator.FetchSynchronizedObject<SynchronizedBreakoutRooms>(conferenceId,
                SynchronizedBreakoutRooms.SyncObjId);
            return GetAvailableScenes(state);
        }

        protected override async ValueTask<bool> InternalIsUpdateRequired(string conferenceId, string roomId,
            string syncObjId, object synchronizedObject, object? previousValue)
        {
            if (synchronizedObject is SynchronizedBreakoutRooms syncBreakoutRooms)
            {
                var isActive = syncBreakoutRooms.Active != null;
                var wasActive = (previousValue as SynchronizedBreakoutRooms)?.Active != null;

                return isActive != wasActive;
            }

            return false;
        }

        private static IEnumerable<IScene> GetAvailableScenes(SynchronizedBreakoutRooms syncBreakoutRooms)
        {
            if (syncBreakoutRooms.Active != null)
                yield return BreakoutRoomScene.Instance;
        }
    }
}
