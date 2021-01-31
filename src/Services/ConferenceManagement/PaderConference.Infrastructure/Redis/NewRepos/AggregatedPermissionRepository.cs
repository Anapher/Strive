using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaderConference.Core.NewServices.Permissions.Gateways;
using PaderConference.Infrastructure.Redis.Repos;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Redis.NewRepos
{
    public class AggregatedPermissionRepository : IAggregatedPermissionRepository, IRedisRepo
    {
        private const string PROPERTY_KEY = "Permissions";

        private readonly IRedisDatabase _redisDatabase;

        public AggregatedPermissionRepository(IRedisDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        public async ValueTask SetPermissions(string conferenceId, string participantId,
            Dictionary<string, JValue> permissions)
        {
            var redisKey = RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId)
                .ForParticipant(participantId).ToString();

            var updated = PermissionDictionaryToHashEntries(permissions);

            await ReplaceHashSet(redisKey, updated);
        }

        private static HashEntry[] PermissionDictionaryToHashEntries(Dictionary<string, JValue> dictionary)
        {
            return dictionary.Select(x => new HashEntry(x.Key, x.Value.ToString(Formatting.None))).ToArray();
        }

        private async Task ReplaceHashSet(string key, HashEntry[] entries)
        {
            var trans = _redisDatabase.Database.CreateTransaction();
            var deleteTask = trans.KeyDeleteAsync(key);
            var _ = trans.HashSetAsync(key, entries);

            await trans.ExecuteAsync();

            await deleteTask; // if the operation failed, this will throw an exception
        }
    }
}
