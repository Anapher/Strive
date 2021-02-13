using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Services.Permissions.Gateways;
using PaderConference.Infrastructure.Redis.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class AggregatedPermissionRepository : IAggregatedPermissionRepository, IRedisRepo
    {
        private const string PROPERTY_KEY = "Permissions";

        private readonly IKeyValueDatabase _database;

        public AggregatedPermissionRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask SetPermissions(string conferenceId, string participantId,
            Dictionary<string, JValue> permissions)
        {
            var redisKey = RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId)
                .ForParticipant(participantId).ToString();

            var updated = PermissionDictionaryToHashEntries(permissions);

            await ReplaceHashSet(redisKey, updated);
        }

        public async ValueTask<T?> GetPermissionsValue<T>(string conferenceId, string participantId, string key)
        {
            var redisKey = RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId)
                .ForParticipant(participantId).ToString();

            return await _database.HashGetAsync<T>(redisKey, key);
        }

        public async ValueTask DeletePermissions(string conferenceId, string participantId)
        {
            var redisKey = RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId)
                .ForParticipant(participantId).ToString();

            await _database.KeyDeleteAsync(redisKey);
        }

        private static IEnumerable<KeyValuePair<string, string>> PermissionDictionaryToHashEntries(
            Dictionary<string, JValue> dictionary)
        {
            return dictionary.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString(Formatting.None)));
        }

        private async Task ReplaceHashSet(string key, IEnumerable<KeyValuePair<string, string>> entries)
        {
            using (var trans = _database.CreateTransaction())
            {
                var deleteTask = trans.KeyDeleteAsync(key);
                var _ = trans.HashSetAsync(key, entries);

                await trans.ExecuteAsync();

                await deleteTask; // if the operation failed, this will throw an exception
            }
        }
    }
}
