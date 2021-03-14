using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PaderConference.Infrastructure.KeyValue.InMemory
{
    public class InMemoryKeyLock : IInMemoryKeyLock
    {
        private readonly object _locksLock = new();
        private readonly Dictionary<string, SemaphoreSlim> _locks = new();

        public Task Lock(string key, CancellationToken token)
        {
            Task acquiredLock;
            lock (_locksLock)
            {
                if (!_locks.TryGetValue(key, out var @lock))
                    _locks[key] = @lock = new SemaphoreSlim(1, 1);

                acquiredLock = @lock.WaitAsync(token);
            }

            return acquiredLock;
        }

        public void Unlock(string key)
        {
            lock (_locksLock)
            {
                if (!_locks.TryGetValue(key, out var @lock)) return;

                @lock.Release();

                if (@lock.Wait(0))
                {
                    @lock.Dispose();
                    _locks.Remove(key);
                }
            }
        }
    }
}
