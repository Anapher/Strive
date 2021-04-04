using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services.Scenes.Modes;

namespace PaderConference.Core.Services.Scenes.Providers
{
    public class DefaultScenesProvider : ISceneProvider
    {
        public ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId)
        {
            return new(new IScene[] {AutonomousScene.Instance, ActiveSpeakerScene.Instance, GridScene.Instance});
        }

        public ValueTask<SceneUpdate> UpdateAvailableScenes(string conferenceId, string roomId,
            object synchronizedObject)
        {
            return new(SceneUpdate.NoUpdateRequired);
        }

        public bool IsProvided(IScene scene)
        {
            return scene is AutonomousScene || scene is ActiveSpeakerScene || scene is GridScene;
        }
    }
}
