using Microsoft.Extensions.Options;
using PaderConference.Infrastructure.KeyValue;
using PaderConference.Infrastructure.KeyValue.InMemory;
using PaderConference.Infrastructure.KeyValue.Redis;
using StackExchange.Redis;

namespace PaderConference.Infrastructure.Redis.Tests.Redis
{
    public static class KeyValueDatabaseFactory
    {
        public static RedisKeyValueDatabase Create(IDatabase database)
        {
            return new(database, new OptionsWrapper<KeyValueDatabaseOptions>(new KeyValueDatabaseOptions()));
        }

        public static InMemoryKeyValueDatabase CreateInMemory()
        {
            return new(new InMemoryKeyValueData(),
                new OptionsWrapper<KeyValueDatabaseOptions>(new KeyValueDatabaseOptions()));
        }
    }
}
