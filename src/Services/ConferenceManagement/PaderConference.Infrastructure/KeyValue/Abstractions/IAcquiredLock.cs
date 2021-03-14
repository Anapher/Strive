using System;
using System.Threading;

namespace PaderConference.Infrastructure.KeyValue.Abstractions
{
    public interface IAcquiredLock : IAsyncDisposable
    {
        CancellationToken HandleLostToken { get; }
    }
}
