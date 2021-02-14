using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Infrastructure.Redis.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class SynchronizedObjectSubscriptionsRepository : ISynchronizedObjectSubscriptionsRepository
    {
        private const string PROPERTY_KEY = "SyncObjectSubscriptions";

        private readonly IKeyValueDatabase _database;

        public SynchronizedObjectSubscriptionsRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask<IReadOnlyList<string>?> GetSet(string conferenceId, string participantId,
            IReadOnlyList<string> subscriptions)
        {
            var key = GetKey(conferenceId);

            using (var transaction = _database.CreateTransaction())
            {
                var previousSubs = transaction.HashGetAsync<List<string>>(key, participantId);
                _ = transaction.HashSetAsync(key, participantId, subscriptions);
                await transaction.ExecuteAsync();

                return await previousSubs;
            }
        }

        public async ValueTask<IReadOnlyList<string>?> Get(string conferenceId, string participantId)
        {
            var key = GetKey(conferenceId);
            return await _database.HashGetAsync<List<string>>(key, participantId);
        }

        public async ValueTask<IReadOnlyList<string>?> Remove(string conferenceId, string participantId)
        {
            var key = GetKey(conferenceId);

            using (var transaction = _database.CreateTransaction())
            {
                var previousSubs = transaction.HashGetAsync<List<string>>(key, participantId);
                _ = transaction.HashDeleteAsync(key, participantId);
                await transaction.ExecuteAsync();

                return await previousSubs;
            }
        }

        public async ValueTask<IReadOnlyDictionary<string, IReadOnlyList<string>?>> GetOfConference(string conferenceId)
        {
            var key = GetKey(conferenceId);
            return await _database.HashGetAllAsync<IReadOnlyList<string>>(key);
        }

        private static string GetKey(string conferenceId)
        {
            return RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
