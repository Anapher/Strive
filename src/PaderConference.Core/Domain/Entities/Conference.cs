using System.Collections.Concurrent;

namespace PaderConference.Core.Domain.Entities
{
    public class Conference
    {
        public Conference(string conferenceId, string initiatorUserId, ConferenceSettings? settings = null)
        {
            ConferenceId = conferenceId;
            InitiatorUserId = initiatorUserId;
            Settings = settings ?? new ConferenceSettings();
        }

        /// <summary>
        ///     The unique conference id
        /// </summary>
        public string ConferenceId { get; }

        /// <summary>
        ///     The user id of the initiator of this conference
        /// </summary>
        public string InitiatorUserId { get; }

        /// <summary>
        ///     The current conference settings
        /// </summary>
        public ConferenceSettings Settings { get; set; }

        /// <summary>
        ///     The current participants of this conference
        /// </summary>
        public ConcurrentDictionary<string, Participant> Participants { get; } =
            new ConcurrentDictionary<string, Participant>();

        protected bool Equals(Conference other)
        {
            return ConferenceId == other.ConferenceId;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Conference) obj);
        }

        public override int GetHashCode()
        {
            return ConferenceId.GetHashCode();
        }
    }
}