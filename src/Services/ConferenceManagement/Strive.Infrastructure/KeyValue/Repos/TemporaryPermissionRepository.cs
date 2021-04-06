using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
        private const string PROPERTY_KEY = "temporaryPermissions";
        private const string LOCK_KEY = "temporaryPermissionsLock";

        private readonly IKeyValueDatabase _redisDatabase;

        public TemporaryPermissionRepository(IKeyValueDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        public async ValueTask SetTemporaryPermission(Participant participant, string key, JValue value)
        {
            var redisKey = GetKey(participant.ConferenceId);
            var lockKey = GetLockKey(participant);

            await using (await _redisDatabase.AcquireLock(lockKey))
            {
                var currentPermissions = await FetchTemporaryPermissions(participant);
                var newPermissions = new Dictionary<string, JValue>(currentPermissions) {[key] = value};
                await _redisDatabase.HashSetAsync(redisKey, participant.Id, newPermissions);
            }
        }

        public async ValueTask RemoveTemporaryPermission(Participant participant, string key)
        {
            var redisKey = GetKey(participant.ConferenceId);
            var lockKey = GetLockKey(participant);

            await using (await _redisDatabase.AcquireLock(lockKey))
            {
                var currentPermissions = await FetchTemporaryPermissions(participant);
                var newPermissions = new Dictionary<string, JValue>(currentPermissions);
                newPermissions.Remove(key);

                if (!newPermissions.Any())
                    await RemoveAllTemporaryPermissions(participant);
                else
                    await _redisDatabase.HashSetAsync(redisKey, participant.Id, newPermissions);
            }
        }

        public async ValueTask<IReadOnlyDictionary<string, JValue>> FetchTemporaryPermissions(Participant participant)
        {
            var redisKey = GetKey(participant.ConferenceId);
            return await _redisDatabase.HashGetAsync<IReadOnlyDictionary<string, JValue>>(redisKey, participant.Id) ??
                   ImmutableDictionary<string, JValue>.Empty;
        }

        public async ValueTask RemoveAllTemporaryPermissions(Participant participant)
        {
            var redisKey = GetKey(participant.ConferenceId);
            await _redisDatabase.HashDeleteAsync(redisKey, participant.Id);
        }

        public async ValueTask<IReadOnlyDictionary<string, IReadOnlyDictionary<string, JValue>>>
            FetchConferenceTemporaryPermissions(string conferenceId)
        {
            var redisKey = GetKey(conferenceId);
            return (await _redisDatabase.HashGetAllAsync<IReadOnlyDictionary<string, JValue>>(redisKey))!;
        }

        public async ValueTask RemoveAllDataOfConference(string conferenceId)
        {
            var key = GetKey(conferenceId);
            await _redisDatabase.KeyDeleteAsync(key);
        }

        private static string GetKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ToString();
        }

        private static string GetLockKey(Participant participant)
        {
            return DatabaseKeyBuilder.ForProperty(LOCK_KEY).ForConference(participant.ConferenceId)
                .ForSecondary(participant.Id).ToString();
        }
    }
}
