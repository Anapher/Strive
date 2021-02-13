using System;
using Medallion.Threading;
using PaderConference.Infrastructure.Redis.Abstractions;

namespace PaderConference.Infrastructure.Redis.InMemory
{
    public class InMemoryInMemoryDatabase : InMemoryDatabaseActions, IKeyValueDatabase
    {
        private readonly InMemoryDatabaseData _data;

        public InMemoryInMemoryDatabase(InMemoryDatabaseData data) : base(data.Data)
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
