using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Strive.Infrastructure.KeyValue.Abstractions;

namespace Strive.Infrastructure.KeyValue.InMemory
{
    public class InMemoryKeyValueDatabase : InMemoryDatabaseActions, IKeyValueDatabase
    {
        private readonly InMemoryKeyValueData _data;
        private readonly KeyValueDatabaseOptions _options;
        private readonly InMemoryKeyLock _lock = new();

        public InMemoryKeyValueDatabase(InMemoryKeyValueData data, IOptions<KeyValueDatabaseOptions> options) :
            base(data.Data)
        {
            _data = data;
            _options = options.Value;
        }

        protected override IDisposable Lock()
        {
            return _data.Lock.WriterLock();
        }

        protected override IDisposable LockRead()
        {
            return _data.Lock.ReaderLock();
        }

        public async ValueTask<IAcquiredLock> AcquireLock(string lockKey, CancellationToken cancellationToken)
        {
            await _lock.Lock(lockKey, cancellationToken);
            return new InMemoryAcquiredLock(lockKey, _lock, _options.LockExpiry);
        }

        public IKeyValueDatabaseTransaction CreateTransaction()
        {
            return new InMemoryDatabaseTransaction(_data);
        }
    }
}
