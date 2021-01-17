using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.Domain.Entities
{
    public class Conference
    {
        public Conference(string conferenceId)
        {
            ConferenceId = conferenceId;
        }

        /// <summary>
        ///     The unique conference id
        /// </summary>
        public string ConferenceId { get; init; }

        /// <summary>
        ///     Conference permission
        /// </summary>
        public Dictionary<PermissionType, Dictionary<string, JValue>> Permissions { get; set; } = new();

        public ConferenceConfiguration Configuration { get; set; } = new();

        public int Version { get; set; }

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
