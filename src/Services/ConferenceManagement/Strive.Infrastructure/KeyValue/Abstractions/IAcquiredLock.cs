using System;
using System.Threading;

namespace Strive.Infrastructure.KeyValue.Abstractions
{
    public interface IAcquiredLock : IAsyncDisposable
    {
        CancellationToken HandleLostToken { get; }
    }
}
