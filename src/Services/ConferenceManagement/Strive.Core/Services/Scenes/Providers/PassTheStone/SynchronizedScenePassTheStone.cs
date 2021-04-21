using System.Collections.Generic;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Scenes.Providers.PassTheStone
{
    public record SynchronizedScenePassTheStone(string? CurrentSpeakerId, IReadOnlyList<string> SpeakerQueue,
        PassTheStoneMode Mode)
    {
        public static readonly SynchronizedObjectId SyncObjId = SynchronizedObjectId.Parse("test");
    }

    public enum PassTheStoneMode
    {
        Race,
        Queue,
        Moderated,
    }
}
