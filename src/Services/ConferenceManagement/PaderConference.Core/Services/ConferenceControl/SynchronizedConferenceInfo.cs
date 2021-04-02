using System;
using System.Collections.Immutable;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.ConferenceControl
{
    public record SynchronizedConferenceInfo(bool IsOpen, IImmutableList<string> Moderators,
        DateTimeOffset? ScheduledDate)
    {
        public static SynchronizedObjectId SyncObjId { get; } = new(SynchronizedObjectIds.CONFERENCE);
    }
}
