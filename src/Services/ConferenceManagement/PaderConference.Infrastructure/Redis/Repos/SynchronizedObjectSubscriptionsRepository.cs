using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Extensions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class SynchronizedObjectSubscriptionsRepository : ISynchronizedObjectSubscriptionsRepository, IRedisRepo
    {
        private const string PROPERTY_KEY = "SyncObjectSubscriptions";

        private readonly IKeyValueDatabase _database;

        public SynchronizedObjectSubscriptionsRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask<IReadOnlyList<string>?> GetSet(Participant participant,
            IReadOnlyList<string> subscriptions)
        {
            var key = GetKey(participant.ConferenceId);

            using (var transaction = _database.CreateTransaction())
            {
                var previousSubs = transaction.HashGetAsync<List<string>>(key, participant.Id);
                _ = transaction.HashSetAsync(key, participant.Id, subscriptions);
                await transaction.ExecuteAsync();

                return await previousSubs;
            }
        }

        public async ValueTask<IReadOnlyList<string>?> Get(Participant participant)
        {
            var key = GetKey(participant.ConferenceId);
            return await _database.HashGetAsync<List<string>>(key, participant.Id);
        }

        public async ValueTask<IReadOnlyList<string>?> Remove(Participant participant)
        {
            var key = GetKey(participant.ConferenceId);

            using (var transaction = _database.CreateTransaction())
            {
                var previousSubs = transaction.HashGetAsync<List<string>>(key, participant.Id);
                _ = transaction.HashDeleteAsync(key, participant.Id);
                await transaction.ExecuteAsync();

                return await previousSubs;
            }
        }

        public async ValueTask<IReadOnlyDictionary<string, IReadOnlyList<string>?>> GetOfConference(string conferenceId)
        {
            var key = GetKey(conferenceId);
            return await _database.HashGetAllAsync<IReadOnlyList<string>>(key);
        }

        public static string GetKey(string conferenceId)
        {
            return RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
