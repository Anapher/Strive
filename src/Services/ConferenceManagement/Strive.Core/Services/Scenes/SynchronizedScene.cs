using System.Collections.Generic;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Scenes
{
    public record SynchronizedScene(IReadOnlyList<IScene> AvailableScenes, IContentScene? OverwrittenContent,
        SceneConfig OverwrittenConfig, IScene SelectedScene, IReadOnlyList<IScene> CurrentSceneStack)
    {
        public const string PROP_ROOMID = "roomId";

        public static SynchronizedObjectId SyncObjId(string roomId)
        {
            return new(SynchronizedObjectIds.SCENE, new Dictionary<string, string> {{PROP_ROOMID, roomId}});
        }
    }
}
