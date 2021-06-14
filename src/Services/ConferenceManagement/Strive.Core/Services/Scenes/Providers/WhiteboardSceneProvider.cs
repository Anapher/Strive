using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Synchronization;
using Strive.Core.Services.Synchronization.Extensions;
using Strive.Core.Services.Whiteboard;

namespace Strive.Core.Services.Scenes.Providers
{
    public class WhiteboardSceneProvider : ContentSceneProvider
    {
        private readonly IMediator _mediator;

        public WhiteboardSceneProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override bool IsProvided(IScene scene)
        {
            return scene is WhiteboardScene;
        }

        public override async ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId,
            IReadOnlyList<IScene> sceneStack)
        {
            var syncObj =
                await _mediator.FetchSynchronizedObject<SynchronizedWhiteboards>(conferenceId,
                    SynchronizedWhiteboards.SyncObjId(roomId));

            return syncObj.Whiteboards.Keys.Select(id => new WhiteboardScene(id));
        }

        protected override async ValueTask<bool> InternalIsUpdateRequired(string conferenceId, string roomId,
            string syncObjId, object synchronizedObject, object? previousValue)
        {
            if (synchronizedObject is SynchronizedWhiteboards)
            {
                var parsedObjId = SynchronizedObjectId.Parse(syncObjId);
                return parsedObjId.Parameters[SynchronizedWhiteboards.ROOM_ID] == roomId;
            }

            return false;
        }
    }
}
