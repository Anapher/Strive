using System.Collections.Generic;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick
{
    public record SynchronizedSceneTalkingStick(string? CurrentSpeakerId, IReadOnlyList<string> SpeakerQueue)
    {
        public static SynchronizedObjectId SyncObjId(string roomId)
        {
            return SynchronizedSceneTalkingStickProvider.BuildSyncObjId(SynchronizedObjectIds.SCENE_TALKINGSTICK,
                roomId);
        }
    }

    public enum TalkingStickMode
    {
        Race,
        Queue,
        Moderated,
        SpeakerPassStick,
    }
}
