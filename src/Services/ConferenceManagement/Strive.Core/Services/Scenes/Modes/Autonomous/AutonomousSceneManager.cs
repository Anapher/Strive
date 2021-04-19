using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Extensions;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Scenes.Scenes;

namespace Strive.Core.Services.Scenes.Modes.Autonomous
{
    public class AutonomousSceneManager : ISceneManager
    {
        public ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId)
        {
            return new(AutonomousScene.Instance.Yield());
        }

        public ValueTask<SceneUpdate> UpdateAvailableScenes(string conferenceId, string roomId,
            object synchronizedObject)
        {
            return new(SceneUpdate.NoUpdateRequired);
        }

        public bool IsProvided(IScene scene)
        {
            return scene is AutonomousScene;
        }

        public ValueTask<IEnumerable<IScene>> BuildStack(IScene scene, SceneBuilderContext context,
            Func<IScene, SceneBuilderContext, IEnumerable<IScene>> sceneProviderFunc)
        {
            var result = new List<IScene> {AutonomousScene.Instance, FindPreferredScene(context)};

            if (context.OverwrittenContent != null)
                result.Add(context.OverwrittenContent);

            return new ValueTask<IEnumerable<IScene>>(result);
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

            return GridScene.Instance;
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
    }
}
