using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services
{
    public abstract class ConferenceService : IConferenceService
    {
        private readonly ConcurrentBag<IAsyncDisposable> _disposables = new();

        public virtual async ValueTask DisposeAsync()
        {
            while (_disposables.TryTake(out var disposable)) await disposable.DisposeAsync();
        }

        public virtual ValueTask InitializeParticipant(Participant participant)
        {
            return new();
        }

        public virtual ValueTask OnClientDisconnected(Participant participant)
        {
            return new();
        }

        public virtual ValueTask OnClientConnected(Participant participant)
        {
            return new();
        }

        public virtual ValueTask InitializeAsync()
        {
            return new();
        }

        public void RegisterDisposable(IAsyncDisposable disposable)
        {
            _disposables.Add(disposable);
        }
    }
}