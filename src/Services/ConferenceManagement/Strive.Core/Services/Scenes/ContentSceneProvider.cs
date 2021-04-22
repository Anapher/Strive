using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Services.Permissions;

namespace Strive.Core.Services.Scenes
{
    public abstract class ContentSceneProvider : ISceneProvider
    {
        public abstract bool IsProvided(IScene scene);

        public abstract ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId,
            IReadOnlyList<IScene> sceneStack);

        public abstract ValueTask<bool> IsUpdateRequired(string conferenceId, string roomId, object synchronizedObject,
            object? previousValue);

        public async ValueTask<IEnumerable<IScene>> BuildStack(IScene scene, SceneBuilderContext context,
            SceneStackFunc sceneProviderFunc)
        {
            var stack = new Stack<IScene>();
            stack.Push(scene);

            return stack;
        }

        public ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(IScene scene,
            Participant participant, IReadOnlyList<IScene> sceneStack)
        {
            return new(Enumerable.Empty<PermissionLayer>());
        }
    }
}
