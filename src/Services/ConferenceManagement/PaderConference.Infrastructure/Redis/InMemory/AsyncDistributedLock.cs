using System;
using System.Threading;
using System.Threading.Tasks;
using Medallion.Threading;
using Nito.AsyncEx;

namespace PaderConference.Infrastructure.Redis.InMemory
{
    public class AsyncDistributedLock : IDistributedLock
    {
        private readonly AsyncReaderWriterLock _lockObject;

        public AsyncDistributedLock(AsyncReaderWriterLock lockObject)
        {
            _lockObject = lockObject;
        }

        public IDistributedSynchronizationHandle TryAcquire(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Acquire(timeout, cancellationToken);
        }

        public IDistributedSynchronizationHandle Acquire(TimeSpan? timeout, CancellationToken cancellationToken)
        {
            var taken = _lockObject.WriterLock(cancellationToken);
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
