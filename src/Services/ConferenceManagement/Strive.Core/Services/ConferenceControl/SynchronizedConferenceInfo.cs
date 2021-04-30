using System;
using System.Collections.Immutable;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.ConferenceControl
{
    public record SynchronizedConferenceInfo(bool IsOpen, IImmutableList<string> Moderators,
        DateTimeOffset? ScheduledDate, string? Name, bool IsPrivateChatEnabled, SceneOptions SceneOptions)
    {
        public static SynchronizedObjectId SyncObjId { get; } = new(SynchronizedObjectIds.CONFERENCE);
    }
}
