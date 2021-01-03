#pragma warning disable 8618
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.Services.Media.Communication
{
    public class SendToConnectionDto
    {
        public JToken Payload { get; set; }

        public string ConnectionId { get; set; }

        public string MethodName { get; set; }
    }
}