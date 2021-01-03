// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

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

        // for deserialization
#pragma warning disable 8618
        // ReSharper disable once UnusedMember.Local
        private Conference()
        {
        }
#pragma warning restore 8618

        /// <summary>
        ///     The unique conference id
        /// </summary>
        public string ConferenceId { get; init; }

        /// <summary>
        ///     Conference permission
        /// </summary>
        public Dictionary<PermissionType, Dictionary<string, JValue>> Permissions { get; set; } = new();

        public ConferenceConfiguration Configuration { get; set; } = new();

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
