using Newtonsoft.Json.Linq;

namespace PaderConference.Messaging.SFU.ReceiveContracts
{
    public interface SendMessageToConnection
    {
        string ConnectionId { get; }

        string MethodName { get; }

        JToken Payload { get; }
    }
}
