using System.Collections.Generic;

namespace Strive.Core.Services.Scenes
{
    public record SceneBuilderContext(string ConferenceId, string RoomId, IEnumerable<IScene> AvailableScenes,
        SceneOptions Options, IReadOnlyList<IScene> PreviousStack);
}
