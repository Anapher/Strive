using System.Text.Json;

namespace PaderConference.Infrastructure.Commander
{
    public class ServiceCommandDto
    {
        public string? Service { get; set; }

        public string? Method { get; set; }

        public JsonElement? Payload { get; set; }
    }
}
