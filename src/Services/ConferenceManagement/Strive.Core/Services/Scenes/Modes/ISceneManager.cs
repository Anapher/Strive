using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Strive.Core.Services.Permissions;

namespace Strive.Core.Services.Scenes.Modes
{
    public interface ISceneManager : ISceneProvider
    {
        ValueTask<IEnumerable<IScene>> BuildStack(IScene scene, SceneBuilderContext context,
            Func<IScene, SceneBuilderContext, IEnumerable<IScene>> sceneProviderFunc);

        ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(IScene scene, Participant participant);
    }

    public record SceneBuilderContext(string ConferenceId, string RoomId, IEnumerable<IScene> AvailableScenes,
        Dictionary<string, JValue> OverwrittenConfig, IReadOnlyList<IScene> PreviousStack,
        IContentScene? OverwrittenContent);
}
