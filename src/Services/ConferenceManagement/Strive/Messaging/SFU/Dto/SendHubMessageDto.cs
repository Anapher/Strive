using Newtonsoft.Json.Linq;

namespace Strive.Messaging.SFU.Dto
{
    public record SendHubMessageDto(string ConnectionId, string MethodName, JToken Payload);
}
