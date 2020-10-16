#pragma warning disable 8618

using System;
using System.Collections.Immutable;

namespace PaderConference.Models.Request
{
    public class CreateConferenceRequestDto
    {
        public string? Name { get; set; }

        public string ConferenceType { get; set; }

        public IImmutableList<string> Organizers { get; set; }

        public DateTimeOffset? StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public string? ScheduleCron { get; set; }

        public IImmutableDictionary<string, string>? Permissions { get; set; }
    }
}