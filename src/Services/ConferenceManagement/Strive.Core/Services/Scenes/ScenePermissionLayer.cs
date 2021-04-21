using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Scenes
{
    public class ScenePermissionLayer : IPermissionLayerProvider
    {
        private readonly IMediator _mediator;
        private readonly IEnumerable<ISceneProvider> _sceneProviders;

        public ScenePermissionLayer(IMediator mediator, IEnumerable<ISceneProvider> sceneProviders)
        {
            _mediator = mediator;
            _sceneProviders = sceneProviders;
        }

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(Participant participant)
        {
            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(participant.ConferenceId,
                SynchronizedRooms.SyncObjId);

            if (!rooms.Participants.TryGetValue(participant.Id, out var roomId))
                return Enumerable.Empty<PermissionLayer>();

            var permissions = new List<PermissionLayer>();

            var scenes = await _mediator.FetchSynchronizedObject<SynchronizedScene>(participant.ConferenceId,
                SynchronizedScene.SyncObjId(roomId));

            foreach (var scene in scenes.CurrentSceneStack)
            {
                var provider = FindProviderForScene(scene);
                var sceneLayers = await provider.FetchPermissionsForParticipant(scene, participant);
                permissions.AddRange(sceneLayers);
            }

            return permissions;
        }

        private ISceneProvider FindProviderForScene(IScene scene)
        {
            return _sceneProviders.First(x => x.IsProvided(scene));
        }
    }
}
