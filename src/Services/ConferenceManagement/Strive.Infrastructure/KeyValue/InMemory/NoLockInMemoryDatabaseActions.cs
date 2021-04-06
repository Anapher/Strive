using System;
using System.Collections.Generic;
using Nito.Disposables;

namespace Strive.Infrastructure.KeyValue.InMemory
{
    public class NoLockInMemoryDatabaseActions : InMemoryDatabaseActions
    {
        public NoLockInMemoryDatabaseActions(Dictionary<string, object> data) : base(data)
        {
        }

        protected override IDisposable Lock()
        {
            return NoopDisposable.Instance;
        }
    }
}
