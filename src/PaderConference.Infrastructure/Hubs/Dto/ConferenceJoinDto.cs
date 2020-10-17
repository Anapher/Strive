using System;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Hubs.Dto
{
    public class ConferenceJoinDto
    {
        public ConferenceState ConferenceState { get; set; }

        public bool Joined { get; set; }

        public DateTimeOffset? ScheduledFor { get; set; }
    }
}