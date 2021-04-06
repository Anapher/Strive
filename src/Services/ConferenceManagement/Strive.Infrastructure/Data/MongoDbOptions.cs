using System.Collections.Generic;

namespace Strive.Infrastructure.Data
{
    public class MongoDbOptions
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";

        public string DatabaseName { get; set; } = "Strive";

        public Dictionary<string, string> CollectionNames { get; set; } = new()
        {
            {"Conference", "Conference"}, {"ConferenceLink", "ConferenceLink"},
        };
    }
}
