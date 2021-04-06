using System;
using System.Linq;

namespace Strive.Core.Domain.Entities
{
    /// <summary>
    ///     A link that a participant has on a conference
    /// </summary>
    public class ConferenceLink : IScheduleInfo
    {
        public ConferenceLink(string participantId, string conferenceId)
        {
            Id = $"{participantId}/{conferenceId}";

            ParticipantId = participantId;
            ConferenceId = conferenceId;
        }

        public string Id { get; init; }

        /// <summary>
        ///     The participant id
        /// </summary>
        public string ParticipantId { get; init; }

        /// <summary>
        ///     The conference id
        /// </summary>
        public string ConferenceId { get; init; }

        /// <summary>
        ///     True if the participant even starred the conference
        /// </summary>
        public bool Starred { get; set; }

        /// <summary>
        ///     The name of the conference
        /// </summary>
        public string? ConferenceName { get; private set; }

        /// <summary>
        ///     True if the participant is a moderator of this conference
        /// </summary>
        public bool IsModerator { get; private set; }

        /// <summary>
        ///     The last time the participant joined this conference
        /// </summary>
        public DateTimeOffset LastJoin { get; private set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? StartTime { get; private set; }

        public string? ScheduleCron { get; private set; }

        public int Version { get; set; }

        public void UpdateFromConference(Conference conference)
        {
            ConferenceName = conference.Configuration.Name;
            IsModerator = conference.Configuration.Moderators.Contains(ParticipantId);
            StartTime = conference.Configuration.StartTime;
            ScheduleCron = conference.Configuration.ScheduleCron;
        }

        public void OnJoined()
        {
            LastJoin = DateTimeOffset.UtcNow;
        }
    }
}
