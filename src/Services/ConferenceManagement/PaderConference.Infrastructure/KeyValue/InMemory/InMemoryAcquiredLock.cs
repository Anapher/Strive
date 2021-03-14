using System;
using System.Threading;
using System.Threading.Tasks;
using PaderConference.Infrastructure.KeyValue.Abstractions;

namespace PaderConference.Infrastructure.KeyValue.InMemory
{
    public class InMemoryAcquiredLock : IAcquiredLock
    {
        private readonly string _key;
        private readonly IInMemoryKeyLock _lockManager;
        private readonly CancellationTokenSource _handleLostTokenSource;
        private readonly object _releaseLock = new();
        private bool _isReleased;

        public InMemoryAcquiredLock(string key, IInMemoryKeyLock lockManager, TimeSpan expiry)
        {
            _key = key;
            _lockManager = lockManager;

            _handleLostTokenSource = new CancellationTokenSource(expiry);
            _handleLostTokenSource.Token.Register(HandleTokenExpired);
        }

        public CancellationToken HandleLostToken => _handleLostTokenSource.Token;

        private void HandleTokenExpired()
        {
            DisposeAsync();
        }

        public ValueTask DisposeAsync()
        {
            if (_isReleased) return new ValueTask();

            lock (_releaseLock)
            {
                if (_isReleased) return new ValueTask();

                _lockManager.Unlock(_key);
                _handleLostTokenSource.Dispose();
                _isReleased = true;
            }

            return new ValueTask();
        }
    }
}
