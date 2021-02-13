using System;
using Medallion.Threading;
using PaderConference.Infrastructure.Redis.Abstractions;

namespace PaderConference.Infrastructure.Redis.InMemory
{
    public class InMemoryKeyValueDatabase : InMemoryDatabaseActions, IKeyValueDatabase
    {
        private readonly InMemoryKeyValueData _data;

        public InMemoryKeyValueDatabase(InMemoryKeyValueData data) : base(data.Data)
        {
            _data = data;
        }

        protected override IDisposable Lock()
        {
            return _data.Lock.Lock();
        }

        public IDistributedLock CreateLock(string lockKey)
        {
            return new AsyncDistributedLock(_data.Lock);
        }

        public IKeyValueDatabaseTransaction CreateTransaction()
        {
            return new InMemoryDatabaseTransaction(_data);
        }
    }
}
