using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Permissions.Gateways;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Extensions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class TemporaryPermissionRepository : ITemporaryPermissionRepository, IRedisRepo
    {
        private const string PROPERTY_KEY = "TemporaryPermissions";

        private readonly IKeyValueDatabase _redisDatabase;

        public TemporaryPermissionRepository(IKeyValueDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        public async ValueTask SetTemporaryPermission(Participant participant, string key, JValue? value)
        {
            var redisKey = GetKey(participant);

            await _redisDatabase.HashSetAsync(redisKey, key, value);
        }

        public async ValueTask RemoveTemporaryPermission(Participant participant, string key)
        {
            var redisKey = GetKey(participant);
            await _redisDatabase.HashDeleteAsync(redisKey, key);
        }

        public async ValueTask<IReadOnlyDictionary<string, JValue>> FetchTemporaryPermissions(Participant participant)
        {
            var redisKey = GetKey(participant);
            return (await _redisDatabase.HashGetAllAsync<JValue>(redisKey))!;
        }

        public async ValueTask RemoveAllTemporaryPermissions(Participant participant)
        {
            var redisKey = GetKey(participant);
            await _redisDatabase.KeyDeleteAsync(redisKey);
        }

        private static string GetKey(Participant participant)
        {
            return RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(participant.ConferenceId)
                .ForSecondary(participant.Id).ToString();
        }
    }
}
