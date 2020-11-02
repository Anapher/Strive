#pragma warning disable 8618
using System.Text.Json;

namespace PaderConference.Core.Services.Media.Communication
{
    public class SendToConnectionDto
    {
        public JsonElement Payload { get; set; }

        public string ConnectionId { get; set; }

        public string MethodName { get; set; }
    }
}