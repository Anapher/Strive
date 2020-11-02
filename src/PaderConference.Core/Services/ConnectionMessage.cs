namespace PaderConference.Core.Services
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
}