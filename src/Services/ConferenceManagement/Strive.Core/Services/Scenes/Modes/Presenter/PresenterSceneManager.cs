using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Strive.Core.Extensions;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Scenes.Scenes;

namespace Strive.Core.Services.Scenes.Modes.Presenter
{
    public class PresenterSceneManager : ISceneManager
    {
        private static readonly IReadOnlyDictionary<string, JValue> PresenterPermissions = new[]
        {
            DefinedPermissions.Media.CanShareAudio.Configure(true),
            DefinedPermissions.Media.CanShareWebcam.Configure(true),
            DefinedPermissions.Media.CanShareScreen.Configure(true),
            DefinedPermissions.Scenes.CanOverwriteContentScene.Configure(true),
        }.ToImmutableDictionary();

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
            return scene is PresenterScene;
        }

        public async ValueTask<IEnumerable<IScene>> BuildStack(IScene scene, SceneBuilderContext context,
            Func<IScene, SceneBuilderContext, IEnumerable<IScene>> sceneProviderFunc)
        {
            var stack = new Stack<IScene>();
            var presenterScene = (PresenterScene) scene;

            var screenShare = context.AvailableScenes.OfType<ScreenShareScene>()
                .FirstOrDefault(x => x.ParticipantId == presenterScene.PresenterParticipantId);
            if (screenShare != null)
            {
                stack.Push(screenShare);
            }
            else
            {
                stack.Push(ActiveSpeakerScene.Instance);
            }

            if (context.OverwrittenContent != null)
                stack.Push(context.OverwrittenContent);

            return stack;
        }

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(IScene scene,
            Participant participant)
        {
            if (scene is PresenterScene presenterScene)
            {
                if (presenterScene.PresenterParticipantId == participant.Id)
                {
                    return new List<PermissionLayer> {CommonPermissionLayers.ScenePresenter(PresenterPermissions)};
                }
            }

            return Enumerable.Empty<PermissionLayer>();
        }
    }
}
