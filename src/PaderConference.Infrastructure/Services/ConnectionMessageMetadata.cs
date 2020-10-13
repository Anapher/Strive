namespace PaderConference.Infrastructure.Services
{
    public class ConnectionMessageMetadata
    {
        public ConnectionMessageMetadata(string conferenceId, string? connectionId, string participantId)
        {
            ConferenceId = conferenceId;
            ConnectionId = connectionId;
            ParticipantId = participantId;
        }

#pragma warning disable 8618
        protected ConnectionMessageMetadata()
        {
        }
#pragma warning restore 8618

        public string ConferenceId { get; set; }

        public string? ConnectionId { get; set; }

        public string ParticipantId { get; set; }
    }
}