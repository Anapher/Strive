using System;
using System.Collections.Immutable;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services.ConferenceControl
{
    public class SynchronizedConferenceInfo
    {
        public SynchronizedConferenceInfo(Conference conference, DateTimeOffset? scheduledDate, bool isOpen)
        {
            ConferenceState = conference.State;
            ScheduledDate = scheduledDate;
            IsOpen = isOpen;
            ConferenceType = conference.ConferenceType;
            Moderators = conference.Moderators;
        }

        public ConferenceState ConferenceState { get; }

        public DateTimeOffset? ScheduledDate { get; }

        public bool IsOpen { get; }

        public string? ConferenceType { get; }

        public IImmutableList<string> Moderators { get; }
    }
}