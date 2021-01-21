using System.Collections.Generic;

namespace PaderConference.Infrastructure.Data
{
    public class MongoDbOptions
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";

        public string DatabaseName { get; set; } = "PaderConference";

        public Dictionary<string, string> CollectionNames { get; set; } = new()
        {
            {"Conference", "Conference"}, {"RefreshToken", "RefreshToken"}, {"ConferenceLink", "ConferenceLink"},
        };
    }
}