using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Extensions;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Scenes.Scenes;

namespace Strive.Core.Services.Scenes.Providers
{
    public class AutonomousSceneManager : ISceneProvider
    {
        public ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId,
            IReadOnlyList<IScene> sceneStack)
        {
            return new(AutonomousScene.Instance.Yield());
        }

        public ValueTask<bool> IsUpdateRequired(string conferenceId, string roomId, object synchronizedObject,
            object? previousValue)
        {
            return new(false);
        }

        public bool IsProvided(IScene scene)
        {
            return scene is AutonomousScene;
        }

        public async ValueTask<IEnumerable<IScene>> BuildStack(IScene scene, SceneBuilderContext context,
            SceneStackFunc sceneProviderFunc)
        {
            return new List<IScene> {AutonomousScene.Instance, FindPreferredScene(context)};
        }

        public ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(IScene scene,
            Participant participant)
        {
            return new(Enumerable.Empty<PermissionLayer>());
        }

        private static IScene FindPreferredScene(SceneBuilderContext context)
        {
            if (TryFindScreenShareScene(context, out var screenShareScene))
                return screenShareScene;

            return GetDefaultScene(context.Options);
        }

        private static bool TryFindScreenShareScene(SceneBuilderContext context,
            [NotNullWhen(true)] out ScreenShareScene? scene)
        {
            var previousScreenShare = context.PreviousStack.OfType<ScreenShareScene>().LastOrDefault();
            if (previousScreenShare != null && context.AvailableScenes.Contains(previousScreenShare))
            {
                scene = previousScreenShare;
                return true;
            }

            scene = context.AvailableScenes.OfType<ScreenShareScene>().OrderBy(x => x.ParticipantId).FirstOrDefault();
            return scene != null;
        }

        private static IScene GetDefaultScene(SceneOptions options)
        {
            switch (options.DefaultScene)
            {
                case SceneOptions.BasicSceneType.Grid:
                    return GridScene.Instance;
                case SceneOptions.BasicSceneType.ActiveSpeaker:
                    return ActiveSpeakerScene.Instance;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options.DefaultScene), options.DefaultScene, null);
            }
        }
    }
}
