using System;
using System.Collections.Immutable;

namespace PaderConference.Core.Services.ConferenceControl
{
    public record SynchronizedConferenceInfo(bool IsOpen, IImmutableList<string> Moderators,
        DateTimeOffset? ScheduledDate);
}
