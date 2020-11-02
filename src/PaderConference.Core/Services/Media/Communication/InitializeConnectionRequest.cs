using System.Text.Json;

namespace PaderConference.Core.Services.Media.Communication
{
    public class InitializeConnectionRequest
    {
        public JsonElement SctpCapabilities { get; set; }

        public JsonElement RtpCapabilities { get; set; }
    }
}