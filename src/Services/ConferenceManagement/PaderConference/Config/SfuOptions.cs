using System;

namespace PaderConference.Config
{
    public class SfuOptions
    {
        public string? ApiKey { get; set; }

        public string SfuPublishExchange { get; set; } = "toSfu";

        public string? UrlTemplate { get; set; }

        public string? TokenSecret { get; set; }

        public TimeSpan TokenExpiration { get; set; } = TimeSpan.FromDays(2);

        public string TokenAudience { get; set; } = "SFU";

        public string TokenIssuer { get; set; } = "ConferenceManagement";
    }
}
