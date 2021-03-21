namespace PaderConference.Messaging.SFU.Contracts
{
    public interface SfuMessage<out T>
    {
        string Type { get; }

        string ConferenceId { get; }

        T Payload { get; }
    }
}
