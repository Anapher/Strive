using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using Strive.Core.Extensions;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Scenes.Modes.PassTheStone
{
    public class PassTheStoneManager : ISceneManager
    {
        private readonly IMediator _mediator;

        private static readonly IReadOnlyDictionary<string, JValue> ParticipantPermissions = new[]
        {
            DefinedPermissions.Media.CanShareAudio.Configure(false),
            DefinedPermissions.Media.CanShareWebcam.Configure(false),
            DefinedPermissions.Media.CanShareScreen.Configure(false),
        }.ToImmutableDictionary();

        public PassTheStoneManager(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId)
        {
            return Enumerable.Empty<IScene>();
        }

        public async ValueTask<SceneUpdate> UpdateAvailableScenes(string conferenceId, string roomId,
            object synchronizedObject)
        {
            return SceneUpdate.NoUpdateRequired;
        }

        public bool IsProvided(IScene scene)
        {
            return scene is PassTheStoneScene;
        }

        public async ValueTask<IEnumerable<IScene>> BuildStack(IScene scene, SceneBuilderContext context,
            Func<IScene, SceneBuilderContext, IEnumerable<IScene>> sceneProviderFunc)
        {
            var stack = new List<IScene>();
            stack.Add(PassTheStoneScene.Instance);

            var syncObj = await GetSyncObj(context.ConferenceId);
            if (syncObj.CurrentSpeakerId != null)
            {
                var presenterScene = new PresenterScene(syncObj.CurrentSpeakerId);
                stack.AddRange(sceneProviderFunc(presenterScene, context));
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
            return (SynchronizedScenePassTheStone) await _mediator.Send(
                new FetchSynchronizedObjectRequest(conferenceId, SynchronizedScenePassTheStone.SyncObjId));
        }
    }
}
