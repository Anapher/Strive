using System;
using System.Collections.Immutable;

namespace PaderConference.Core.Domain.Entities
{
    public class Participant
    {
        public Participant(string participantId, string? displayName, string role, DateTimeOffset timestamp)
        {
            ParticipantId = participantId;
            DisplayName = displayName;
            Role = role;
            Timestamp = timestamp;
        }

        public string ParticipantId { get; }

        public string? DisplayName { get; }

        public string Role { get; }

        public IImmutableDictionary<string, string> Attributes { get; set; } =
            ImmutableDictionary<string, string>.Empty;

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