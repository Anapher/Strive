using System;
using System.Threading.Tasks;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Infrastructure.Redis.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class SynchronizedObjectRepository : ISynchronizedObjectRepository, IRedisRepo
    {
        private const string PROPERTY_KEY = "SyncObject";
        private const string LOCK_KEY = "SyncObjectLock";

        private readonly IKeyValueDatabase _database;

        public SynchronizedObjectRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async Task<T?> Update<T>(string conferenceId, string name, T value)
        {
            var redisKey = RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ForParticipant(name)
                .ToString();

            var previous = await _database.GetSetAsync(redisKey, value);
            return previous;
        }

        public async Task<(T? previousValue, T newValue)> Update<T>(string conferenceId, string name,
            Func<T?, T> updateValueFn)
        {
            var redisKey = RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ForParticipant(name)
                .ToString();

            var lockKey = RedisKeyBuilder.ForProperty(LOCK_KEY).ForConference(conferenceId).ForParticipant(name)
                .ToString();

            var @lock = _database.CreateLock(lockKey);
            await using (await @lock.AcquireAsync())
            {
                var currentValue = await _database.GetAsync<T>(redisKey);

                var newValue = updateValueFn(currentValue);

                await _database.SetAsync(redisKey, newValue);
                return (currentValue, newValue);
            }
        }
    }
}
