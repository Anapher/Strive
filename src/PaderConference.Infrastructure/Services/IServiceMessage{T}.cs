namespace PaderConference.Infrastructure.Services
{
    public interface IServiceMessage<out TPayload> : IServiceMessage
    {
        TPayload Payload { get; }
    }
}