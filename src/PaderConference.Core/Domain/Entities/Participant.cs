using System;

namespace PaderConference.Core.Domain.Entities
{
    public class Participant
    {
        public Participant(string participantId, string? displayName, DateTimeOffset timestamp)
        {
            ParticipantId = participantId;
            DisplayName = displayName;
            Timestamp = timestamp;
        }

        public string ParticipantId { get; }

        public string? DisplayName { get; }

        public DateTimeOffset Timestamp { get; }
    }
}