using System.Collections.Generic;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Scenes
{
    public record SynchronizedScene(ActiveScene Active, IReadOnlyList<IScene> AvailableScenes)
    {
        public const string PROP_ROOMID = "roomId";

        public static SynchronizedObjectId SyncObjId(string roomId)
        {
            return new(SynchronizedObjectIds.SCENE, new Dictionary<string, string> {{PROP_ROOMID, roomId}});
        }
    }
}
