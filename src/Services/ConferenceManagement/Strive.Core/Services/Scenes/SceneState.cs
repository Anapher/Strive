using System.Collections.Generic;

namespace Strive.Core.Services.Scenes
{
    public record SceneState(IReadOnlyList<IScene> SceneStack, IReadOnlyList<IScene> AvailableScenes);
}
