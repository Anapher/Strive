using System;

namespace PaderConference.Core.Domain.Entities
{
    public class Participant
    {
        public Participant(string participantId, string? displayName, DateTimeOffset timestamp, Conference conference)
        {
            ParticipantId = participantId;
            DisplayName = displayName;
            Timestamp = timestamp;
            Conference = conference;
        }

        public Conference Conference { get; }

        public string ParticipantId { get; }

        public string? DisplayName { get; }

        public DateTimeOffset Timestamp { get; }

        protected bool Equals(Participant other)
        {
            return ParticipantId == other.ParticipantId;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Participant) obj);
        }

        public override int GetHashCode()
        {
            return ParticipantId.GetHashCode();
        }
    }
}