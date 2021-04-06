using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Strive.Core.Services;
using Strive.Core.Services.Permissions.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Extensions;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class TemporaryPermissionRepository : ITemporaryPermissionRepository, IKeyValueRepo
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
            return DatabaseKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(participant.ConferenceId)
                .ForSecondary(participant.Id).ToString();
        }
    }
}
