using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Strive.Core.Services.Scenes
{
    public interface ISceneProvider
    {
        ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId);

        ValueTask<SceneUpdate> UpdateAvailableScenes(string conferenceId, string roomId, object synchronizedObject);

        bool IsProvided(IScene scene);
    }

    public record SceneUpdate
    {
        private SceneUpdate(IEnumerable<IScene>? updateScenes)
        {
            Required = updateScenes != null;
            UpdateScenes = updateScenes;
        }

        [MemberNotNullWhen(true, nameof(UpdateScenes))]
        public bool Required { get; }

        public IEnumerable<IScene>? UpdateScenes { get; }

        public static readonly SceneUpdate NoUpdateRequired = new((IEnumerable<IScene>?) null);

        public static SceneUpdate UpdateRequired(IEnumerable<IScene> updatedScenes)
        {
            return new(updatedScenes);
        }
    }

    public enum SceneValidationResult
    {
        None,
        Valid,
        Invalid,
    }
}
