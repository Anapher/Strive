using System.Threading.Tasks;
using Strive.Infrastructure.KeyValue.Abstractions;
using StackExchange.Redis;

namespace Strive.Infrastructure.KeyValue.Redis
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
