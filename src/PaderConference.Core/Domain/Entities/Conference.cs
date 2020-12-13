// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

using System;
using System.Collections.Immutable;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Core.Domain.Entities
{
    public class Conference : IConferenceScheduleInfo
    {
        public Conference(string conferenceId, IImmutableList<string> moderators)
        {
            ConferenceId = conferenceId;
            Moderators = moderators;
        }

#pragma warning disable 8618
        // ReSharper disable once UnusedMember.Local
        private Conference()
        {
        }
#pragma warning restore 8618

        /// <summary>
        ///     The name of the conference. May be null
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        ///     Participant ids of organizers
        /// </summary>
        public IImmutableList<string> Moderators { get; set; }

        /// <summary>
        ///     Conference permission
        /// </summary>
        public IImmutableDictionary<string, string>? Permissions { get; set; }

        /// <summary>
        ///     Permissions of moderators
        /// </summary>
        public IImmutableDictionary<string, string>? ModeratorPermissions { get; set; }

        /// <summary>
        ///     Default permissions for rooms
        /// </summary>
        public IImmutableDictionary<string, string>? DefaultRoomPermissions { get; set; }

        /// <summary>
        ///     The type of the conference. Set by the front end to synchronize devices
        /// </summary>
        public string? ConferenceType { get; set; }

        /// <summary>
        ///     Conference state
        /// </summary>
        public ConferenceState State { get; set; } = ConferenceState.Active;

        /// <summary>
        ///     The unique conference id
        /// </summary>
        public string ConferenceId { get; private set; }

        /// <summary>
        ///     The starting time of this conference. If <see cref="ScheduleCron" /> is not null, this is the first time the
        ///     conference starts
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        ///     A cron string that determines the scheduled time of this conference. The schedule starts after
        ///     <see cref="StartTime" />
        /// </summary>
        public string? ScheduleCron { get; set; }

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
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return ConferenceId.GetHashCode();
        }
    }

    public enum ConferenceState
    {
        Active,
        Inactive,
    }
}
