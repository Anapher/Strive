namespace PaderConference.Infrastructure.Services.Media.Communication
{
    public class ConnectionMessage<TPayload>
    {
        public ConnectionMessage(TPayload payload, ConnectionMessageMetadata meta)
        {
            Payload = payload;
            Meta = meta;
        }

#pragma warning disable 8618
        private ConnectionMessage()
        {
        }
#pragma warning restore 8618

        public ConnectionMessageMetadata Meta { get; set; }

        public TPayload Payload { get; set; }
    }

    public class ConnectionMessageMetadata
    {
        public ConnectionMessageMetadata(string conferenceId, string connectionId, string participantId)
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

        public string ConnectionId { get; set; }

        public string ParticipantId { get; set; }
    }
}