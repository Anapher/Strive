using System.Threading;
using System.Threading.Tasks;
using Medallion.Threading.Redis;
using Microsoft.Extensions.Options;
using Strive.Infrastructure.KeyValue.Abstractions;
using StackExchange.Redis;

namespace Strive.Infrastructure.KeyValue.Redis
{
    public class RedisKeyValueDatabase : RedisActions, IKeyValueDatabase
    {
        private readonly IDatabase _database;
        private readonly KeyValueDatabaseOptions _options;

        public RedisKeyValueDatabase(IDatabase database, IOptions<KeyValueDatabaseOptions> options) : base(database)
        {
            _database = database;
            _options = options.Value;
        }

        public IKeyValueDatabaseTransaction CreateTransaction()
        {
            var transaction = _database.CreateTransaction();
            return new RedisTransaction(transaction);
        }

        public async ValueTask<IAcquiredLock> AcquireLock(string lockKey, CancellationToken cancellationToken)
        {
            var @lock = new RedisDistributedLock(lockKey, _database, builder => builder.Expiry(_options.LockExpiry));
            var acquired = await @lock.AcquireAsync(null, cancellationToken);
            return new RedisAcquiredLockWrapper(acquired);
        }
    }
}
