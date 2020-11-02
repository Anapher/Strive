namespace PaderConference.Core.Services
{
    public interface IServiceMessage<out TPayload> : IServiceMessage
    {
        TPayload Payload { get; }
    }
}