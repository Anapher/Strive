using StackExchange.Redis.Extensions.Core.Configuration;

namespace PaderConference.Config
{
    public class KeyValueDatabaseOptions
    {
        public bool UseInMemory { get; set; }

        public RedisConfiguration? Redis { get; set; }
    }
}
