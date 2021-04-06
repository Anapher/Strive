using System.Threading;
using System.Threading.Tasks;

namespace Strive.Infrastructure.KeyValue.Abstractions
{
    public interface IKeyValueDatabase : IKeyValueDatabaseActions
    {
        IKeyValueDatabaseTransaction CreateTransaction();

        ValueTask<IAcquiredLock> AcquireLock(string lockKey, CancellationToken cancellationToken = default);
    }
}
