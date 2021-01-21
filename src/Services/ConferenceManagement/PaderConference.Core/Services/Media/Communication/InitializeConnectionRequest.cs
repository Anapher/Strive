#pragma warning disable 8618
using Newtonsoft.Json.Linq;

namespace PaderConference.Core.Services.Media.Communication
{
    public class InitializeConnectionRequest
    {
        public JObject SctpCapabilities { get; set; }

        public JObject RtpCapabilities { get; set; }
    }
}