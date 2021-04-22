using System.Collections.Generic;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick
{
    public record SynchronizedSceneTalkingStick(string? CurrentSpeakerId, IReadOnlyList<string> SpeakerQueue,
        TalkingStickMode Mode)
    {
        public static SynchronizedObjectId SyncObjId(string roomId)
        {
            return SynchronizedObjectId.Parse("test");
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
