using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Services;
using Strive.Core.Services.Synchronization.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Extensions;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class SynchronizedObjectSubscriptionsRepository : ISynchronizedObjectSubscriptionsRepository, IKeyValueRepo
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
            return DatabaseKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
