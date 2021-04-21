using System.Collections.Generic;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Scenes
{
    public record SynchronizedScene(IScene SelectedScene, IScene? OverwrittenContent,
        IReadOnlyList<IScene> AvailableScenes, IReadOnlyList<IScene> CurrentSceneStack)
    {
        public static SynchronizedObjectId SyncObjId(string roomId)
        {
            return SynchronizedSceneProvider.BuildSyncObjId(SynchronizedObjectIds.SCENE, roomId);
        }
    }
}
