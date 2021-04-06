using Microsoft.Extensions.Options;
using Strive.Infrastructure.KeyValue;
using Strive.Infrastructure.KeyValue.InMemory;
using Strive.Infrastructure.KeyValue.Redis;
using StackExchange.Redis;

namespace Strive.Infrastructure.Redis.Tests.Redis
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
