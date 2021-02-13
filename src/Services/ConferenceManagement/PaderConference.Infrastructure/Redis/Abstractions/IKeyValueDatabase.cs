using Medallion.Threading;

namespace PaderConference.Infrastructure.Redis.Abstractions
{
    public interface IKeyValueDatabase : IKeyValueDatabaseActions
    {
        IKeyValueDatabaseTransaction CreateTransaction();

        IDistributedLock CreateLock(string lockKey);
    }
}
