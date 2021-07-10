using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Services.Scenes.Scenes;

namespace Strive.Core.Services.Scenes.Providers
{
    public class DefaultScenesProvider : ContentSceneProvider
    {
        public override ValueTask<IEnumerable<IScene>> GetAvailableScenes(string conferenceId, string roomId,
            IReadOnlyList<IScene> sceneStack)
        {
            return new(new IScene[] {ActiveSpeakerScene.Instance, GridScene.Instance});
        }

        protected override ValueTask<bool> InternalIsUpdateRequired(string conferenceId, string roomId,
            string syncObjId, object synchronizedObject, object? previousValue)
        {
            return new(false);
        }

        public override bool IsProvided(IScene scene)
        {
            return scene is ActiveSpeakerScene or GridScene;
        }
    }
}
