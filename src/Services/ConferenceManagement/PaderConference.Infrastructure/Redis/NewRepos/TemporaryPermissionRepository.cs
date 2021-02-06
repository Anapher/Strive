using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Services.Permissions.Gateways;
using PaderConference.Infrastructure.Redis.Repos;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Redis.NewRepos
{
    public class TemporaryPermissionRepository : ITemporaryPermissionRepository, IRedisRepo
    {
        private const string PROPERTY_KEY = "TemporaryPermissions";

        private readonly IRedisDatabase _redisDatabase;

        public TemporaryPermissionRepository(IRedisDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        public async ValueTask SetTemporaryPermission(string conferenceId, string participantId, string key,
            JValue? value)
        {
            var redisKey = RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId)
                .ForParticipant(participantId).ToString();

            await _redisDatabase.HashSetAsync(redisKey, key, value);
        }

        public async ValueTask RemoveTemporaryPermission(string conferenceId, string participantId, string key)
        {
            var redisKey = RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId)
                .ForParticipant(participantId).ToString();

            await _redisDatabase.HashDeleteAsync(redisKey, key);
        }

        public async ValueTask<Dictionary<string, JValue>> FetchTemporaryPermissions(string conferenceId,
            string participantId)
        {
            var redisKey = RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId)
                .ForParticipant(participantId).ToString();

            return await _redisDatabase.HashGetAllAsync<JValue>(redisKey);
        }
    }
}
