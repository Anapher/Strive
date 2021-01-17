using System;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Models.Response
{
    public class ConferenceLinkDto
    {
        public ConferenceLinkDto(ConferenceLink link, bool isActive, DateTimeOffset? scheduled)
        {
            ConferenceId = link.ConferenceId;
            IsActive = isActive;
            ConferenceName = link.ConferenceName;
            Starred = link.Starred;
            IsModerator = link.IsModerator;
            LastJoin = link.LastJoin;
            Scheduled = scheduled;
        }

        public string ConferenceId { get; set; }

        public bool IsActive { get; set; }

        public string? ConferenceName { get; set; }

        public bool Starred { get; set; }

        public bool IsModerator { get; set; }

        public DateTimeOffset LastJoin { get; set; }

        public DateTimeOffset? Scheduled { get; set; }
    }
}
