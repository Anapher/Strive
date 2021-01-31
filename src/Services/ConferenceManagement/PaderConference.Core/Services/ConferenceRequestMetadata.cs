using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services
{
    public class ConferenceRequestMetadata
    {
        public ConferenceRequestMetadata(string conferenceId, Participant sender, string connectionId)
        {
            ConferenceId = conferenceId;
            Sender = sender;
            ConnectionId = connectionId;
        }

        public string ConferenceId { get; }
        public Participant Sender { get; }
        public string ConnectionId { get; }
    }
}
