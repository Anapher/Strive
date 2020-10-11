using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services.Media.Communication
{
    public class ConferenceInfo
    {
        public ConferenceInfo(Conference conference)
        {
            Id = conference.ConferenceId;
        }

        public string Id { get; set; }
    }
}