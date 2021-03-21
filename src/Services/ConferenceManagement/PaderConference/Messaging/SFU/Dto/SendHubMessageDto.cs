using Newtonsoft.Json.Linq;

namespace PaderConference.Messaging.SFU.Dto
{
    public record SendHubMessageDto(string ConnectionId, string MethodName, JToken Payload);
}
