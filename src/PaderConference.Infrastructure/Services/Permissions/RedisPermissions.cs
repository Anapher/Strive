using System.Threading.Tasks;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public class RedisPermissions : IPermissionStack
    {
        private readonly IRedisDatabase _database;
        private readonly string _redisKey;

        public RedisPermissions(IRedisDatabase database, string redisKey)
        {
            _database = database;
            _redisKey = redisKey;
        }

        public async ValueTask<T> GetPermission<T>(PermissionDescriptor<T> descriptor) =>
            await _database.HashGetAsync<T>(_redisKey, descriptor.Key) ?? (T) descriptor.DefaultValue;
    }
}
