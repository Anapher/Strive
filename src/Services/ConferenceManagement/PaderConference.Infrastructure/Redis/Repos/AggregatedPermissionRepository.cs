using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Permissions.Gateways;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Extensions;

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

        public async ValueTask SetPermissions(Participant participant, Dictionary<string, JValue> permissions)
        {
            var redisKey = GetPermissionsKey(participant);
            var updated = PermissionDictionaryToHashEntries(permissions);

            await ReplaceHashSet(redisKey, updated);
        }

        public async ValueTask<T?> GetPermissionsValue<T>(Participant participant, string key)
        {
            var redisKey = GetPermissionsKey(participant);
            return await _database.HashGetAsync<T>(redisKey, key);
        }

        public async ValueTask<Dictionary<string, JValue>> GetPermissions(Participant participant)
        {
            var redisKey = GetPermissionsKey(participant);
            var dictionary = await _database.HashGetAllAsync(redisKey);
            return dictionary.ToDictionary(x => x.Key, x => (JValue) JToken.Parse(x.Value));
        }

        public async ValueTask DeletePermissions(Participant participant)
        {
            var redisKey = GetPermissionsKey(participant);
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

        private static string GetPermissionsKey(Participant participant)
        {
            return RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(participant.ConferenceId)
                .ForSecondary(participant.Id).ToString();
        }
    }
}
