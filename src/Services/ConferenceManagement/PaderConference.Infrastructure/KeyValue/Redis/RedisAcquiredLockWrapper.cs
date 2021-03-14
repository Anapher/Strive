using System.Threading;
using System.Threading.Tasks;
using Medallion.Threading.Redis;
using PaderConference.Infrastructure.KeyValue.Abstractions;

namespace PaderConference.Infrastructure.KeyValue.Redis
{
    public class RedisAcquiredLockWrapper : IAcquiredLock
    {
        private readonly RedisDistributedLockHandle _handle;

        public RedisAcquiredLockWrapper(RedisDistributedLockHandle handle)
        {
            _handle = handle;
        }

        public ValueTask DisposeAsync()
        {
            return _handle.DisposeAsync();
        }

        public CancellationToken HandleLostToken => _handle.HandleLostToken;
    }
}
