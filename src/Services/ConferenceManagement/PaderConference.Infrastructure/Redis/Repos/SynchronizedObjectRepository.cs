using System;
using System.Threading.Tasks;
using Medallion.Threading.Redis;
using PaderConference.Core.Services.Synchronization.Gateways;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class SynchronizedObjectRepository : ISynchronizedObjectRepository, IRedisRepo
    {
        private const string PROPERTY_KEY = "SyncObject";
        private const string LOCK_KEY = "SyncObjectLock";

        private readonly IRedisDatabase _redisDatabase;

        public SynchronizedObjectRepository(IRedisDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        public async Task<T?> Update<T>(string conferenceId, string name, T value)
        {
            var redisKey = RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ForParticipant(name)
                .ToString();

            var data = RedisSerializer.SerializeValue(value);

            var previous = await _redisDatabase.Database.StringGetSetAsync(redisKey, data);
            return RedisSerializer.DeserializeValue<T>(previous);
        }

        public async Task<(T? previousValue, T newValue)> Update<T>(string conferenceId, string name,
            Func<T?, T> updateValueFn)
        {
            var redisKey = RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ForParticipant(name)
                .ToString();

            var lockKey = RedisKeyBuilder.ForProperty(LOCK_KEY).ForConference(conferenceId).ForParticipant(name)
                .ToString();

            var @lock = new RedisDistributedLock(lockKey, _redisDatabase.Database,
                builder => builder.Expiry(TimeSpan.FromSeconds(5)));

            await using (await @lock.AcquireAsync())
            {
                var currentData = await _redisDatabase.Database.StringGetAsync(redisKey);
                var currentValue = RedisSerializer.DeserializeValue<T>(currentData);

                var newValue = updateValueFn(currentValue);
                var newData = RedisSerializer.SerializeValue(newValue);

                await _redisDatabase.Database.StringSetAsync(redisKey, newData);
                return (currentValue, newValue);
            }
        }
    }
}
