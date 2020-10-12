// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

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

#pragma warning disable 8618
        // ReSharper disable once UnusedMember.Local
        private Conference()
        {
        }
#pragma warning restore 8618

        /// <summary>
        ///     The unique conference id
        /// </summary>
        public string ConferenceId { get; private set; }

        /// <summary>
        ///     The user id of the initiator of this conference
        /// </summary>
        public string InitiatorUserId { get; private set; }

        /// <summary>
        ///     The current conference settings
        /// </summary>
        public ConferenceSettings Settings { get; private set; }

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