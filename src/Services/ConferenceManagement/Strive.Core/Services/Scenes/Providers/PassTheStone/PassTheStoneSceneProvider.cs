using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using Strive.Core.Extensions;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Scenes.Providers.PassTheStone
{
    public class PassTheStoneSceneProvider : ISceneProvider
    {
        private readonly IMediator _mediator;

        private static readonly IReadOnlyDictionary<string, JValue> ParticipantPermissions = new[]
        {
            DefinedPermissions.Media.CanShareAudio.Configure(false),
            DefinedPermissions.Media.CanShareWebcam.Configure(false),
            DefinedPermissions.Media.CanShareScreen.Configure(false),
        }.ToImmutableDictionary();

        public PassTheStoneSceneProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId,
            IReadOnlyList<IScene> sceneStack)
        {
            if (sceneStack.OfType<PassTheStoneScene>().Any())
                return PassTheStoneScene.Instance.Yield();

            return Enumerable.Empty<IScene>();
        }

        public ValueTask<bool> IsUpdateRequired(string conferenceId, string roomId, object synchronizedObject,
            object? previousValue)
        {
            return new(false);
        }

        public bool IsProvided(IScene scene)
        {
            return scene is PassTheStoneScene;
        }

        public async ValueTask<IEnumerable<IScene>> BuildStack(IScene scene, SceneBuilderContext context,
            SceneStackFunc sceneProviderFunc)
        {
            var stack = new List<IScene> {PassTheStoneScene.Instance};

            var syncObj = await GetSyncObj(context.ConferenceId);
            if (syncObj.CurrentSpeakerId != null)
            {
                var presenterScene = new PresenterScene(syncObj.CurrentSpeakerId);
                stack.AddRange(await sceneProviderFunc(presenterScene, context));
            }

            return stack;
        }

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(IScene scene,
            Participant participant)
        {
            if (scene is PassTheStoneScene)
            {
                return CommonPermissionLayers.ScenePassTheStone(ParticipantPermissions).Yield();
            }

            return Enumerable.Empty<PermissionLayer>();
        }

        private async ValueTask<SynchronizedScenePassTheStone> GetSyncObj(string conferenceId)
        {
            return await _mediator.FetchSynchronizedObject<SynchronizedScenePassTheStone>(conferenceId,
                SynchronizedScenePassTheStone.SyncObjId);
        }
    }
}
