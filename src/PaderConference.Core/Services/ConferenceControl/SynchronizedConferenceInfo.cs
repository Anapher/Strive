using System;
using System.Collections.Immutable;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services.ConferenceControl
{
    public record SynchronizedConferenceInfo
    {
        public SynchronizedConferenceInfo(Conference conference)
        {
            ConferenceType = conference.Configuration.ConferenceType;
            Moderators = conference.Configuration.Moderators;
        }

        public DateTimeOffset? ScheduledDate { get; init; }

        public bool IsOpen { get; init; }

        public string? ConferenceType { get; }

        public IImmutableList<string> Moderators { get; }
    }
}