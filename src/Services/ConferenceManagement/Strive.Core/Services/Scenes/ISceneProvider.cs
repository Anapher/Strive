using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Services.Permissions;

namespace Strive.Core.Services.Scenes
{
    public delegate ValueTask<IEnumerable<IScene>> SceneStackFunc(IScene scene, SceneBuilderContext context);

    public interface ISceneProvider
    {
        bool IsProvided(IScene scene);

        ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId,
            IReadOnlyList<IScene> sceneStack);

        ValueTask<bool> IsUpdateRequired(string conferenceId, string roomId, object synchronizedObject,
            object? previousValue);

        ValueTask<IEnumerable<IScene>> BuildStack(IScene scene, SceneBuilderContext context,
            SceneStackFunc sceneProviderFunc);

        ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(IScene scene, Participant participant);
    }
}
