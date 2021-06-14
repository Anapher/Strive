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

        ValueTask<SceneUpdate> IsUpdateRequired(string conferenceId, string roomId, string syncObjId,
            object synchronizedObject, object? previousValue);

        ValueTask<IEnumerable<IScene>> BuildStack(IScene scene, SceneBuilderContext context,
            SceneStackFunc sceneProviderFunc);

        ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(IScene scene, Participant participant,
            IReadOnlyList<IScene> sceneStack);
    }

    public enum SceneUpdate
    {
        NotRequired,
        AvailableScenesChanged,
        SceneStackChanged,
    }
}
