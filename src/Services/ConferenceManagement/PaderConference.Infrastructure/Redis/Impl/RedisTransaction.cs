using System.Threading.Tasks;
using PaderConference.Infrastructure.Redis.Abstractions;
using StackExchange.Redis;

namespace PaderConference.Infrastructure.Redis.Impl
{
    public class RedisTransaction : RedisActions, IKeyValueDatabaseTransaction
    {
        private readonly ITransaction _transaction;

        public RedisTransaction(ITransaction transaction) : base(transaction)
        {
            _transaction = transaction;
        }

        public void Dispose()
        {
        }

        public async ValueTask<bool> ExecuteAsync()
        {
            return await _transaction.ExecuteAsync();
        }
    }
}
