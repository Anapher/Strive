#pragma warning disable 8618
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Models.Request
{
    public class StartConferenceRequestDto
    {
        public ConferenceSettings? Settings { get; set; }
    }
}