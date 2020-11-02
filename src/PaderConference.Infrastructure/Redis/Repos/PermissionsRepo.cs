using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class PermissionsRepo : IRedisRepo, IPermissionsRepo
    {
        private readonly IRedisDatabase _database;

        public PermissionsRepo(IRedisDatabase database)
        {
            _database = database;
        }

        public Task<T> GetPermissionsValue<T>(string participantId, string key)
        {
            return _database.HashGetAsync<T>(RedisKeys.ParticipantPermissions(participantId), key);
        }

        public Task SetPermissions(string participantId, Dictionary<string, JsonElement> permissions)
        {
            return _database.HashSetAsync(RedisKeys.ParticipantPermissions(participantId), permissions);
        }

        public Task PublishPermissionsUpdated(string[] participantIds)
        {
            return _database.PublishAsync(RedisChannels.OnPermissionsUpdated, participantIds);
        }
    }
}
