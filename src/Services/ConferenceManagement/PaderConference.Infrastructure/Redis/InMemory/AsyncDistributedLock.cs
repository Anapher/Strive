using System;
using System.Threading;
using System.Threading.Tasks;
using Medallion.Threading;
using Nito.AsyncEx;

namespace PaderConference.Infrastructure.Redis.InMemory
{
    public class AsyncDistributedLock : IDistributedLock
    {
        private readonly AsyncLock _lockObject;

        public AsyncDistributedLock(AsyncLock lockObject)
        {
            _lockObject = lockObject;
        }

        public IDistributedSynchronizationHandle TryAcquire(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Acquire(timeout, cancellationToken);
        }

        public IDistributedSynchronizationHandle Acquire(TimeSpan? timeout, CancellationToken cancellationToken)
        {
            var taken = _lockObject.Lock(cancellationToken);
            return new AsyncLockDistributedHandle(taken);
        }

        public ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            return new(TryAcquire(timeout, cancellationToken));
        }

        public ValueTask<IDistributedSynchronizationHandle> AcquireAsync(TimeSpan? timeout,
            CancellationToken cancellationToken)
        {
            return new(Acquire(timeout, cancellationToken));
        }

        public string Name { get; } = nameof(AsyncDistributedLock);
    }
}
