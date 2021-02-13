using System;
using Medallion.Threading;
using Medallion.Threading.Redis;
using PaderConference.Infrastructure.Redis.Abstractions;
using StackExchange.Redis;

namespace PaderConference.Infrastructure.Redis.Impl
{
    public class RedisKeyValueDatabase : RedisActions, IKeyValueDatabase
    {
        private readonly IDatabase _database;

        public RedisKeyValueDatabase(IDatabase database) : base(database)
        {
            _database = database;
        }

        public IKeyValueDatabaseTransaction CreateTransaction()
        {
            var transaction = _database.CreateTransaction();
            return new RedisTransaction(transaction, _database);
        }

        public override IDistributedLock CreateLock(string lockKey)
        {
            return new RedisDistributedLock(lockKey, _database, builder => builder.Expiry(TimeSpan.FromSeconds(5)));
        }
    }
}
