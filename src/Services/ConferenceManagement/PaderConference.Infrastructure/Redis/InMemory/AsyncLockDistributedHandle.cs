using System;
using System.Threading;
using System.Threading.Tasks;
using Medallion.Threading;

namespace PaderConference.Infrastructure.Redis.InMemory
{
    public class AsyncLockDistributedHandle : IDistributedSynchronizationHandle
    {
        private readonly IDisposable _asyncLockHandler;

        public AsyncLockDistributedHandle(IDisposable asyncLockHandler)
        {
            _asyncLockHandler = asyncLockHandler;
        }

        public void Dispose()
        {
            _asyncLockHandler.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return new ValueTask();
        }

        public CancellationToken HandleLostToken { get; } = CancellationToken.None;
    }
}
