using System;
using System.Threading.Tasks;
using Medallion.Threading;
using Medallion.Threading.Redis;
using PaderConference.Infrastructure.Redis.Abstractions;
using StackExchange.Redis;

namespace PaderConference.Infrastructure.Redis.Impl
{
    public class RedisTransaction : RedisActions, IKeyValueDatabaseTransaction
    {
        private readonly ITransaction _transaction;
        private readonly IDatabase _database;

        public RedisTransaction(ITransaction transaction, IDatabase database) : base(transaction)
        {
            _transaction = transaction;
            _database = database;
        }

        public void Dispose()
        {
        }

        public async ValueTask<bool> ExecuteAsync()
        {
            return await _transaction.ExecuteAsync();
        }

        public override IDistributedLock CreateLock(string lockKey)
        {
            return new RedisDistributedLock(lockKey, _database, builder => builder.Expiry(TimeSpan.FromSeconds(5)));
        }
    }
}
