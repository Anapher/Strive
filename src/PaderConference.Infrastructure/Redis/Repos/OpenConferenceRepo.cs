using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class OpenConferenceRepo : IRedisRepo, IOpenConferenceRepo
    {
        private readonly IRedisDatabase _database;

        public OpenConferenceRepo(IRedisDatabase database)
        {
            _database = database;
        }

        public async Task DeleteAll()
        {
            await _database.PublishAsync<object?>(RedisChannels.OnResetConferences, null);
            await _database.RemoveAsync(RedisKeys.OpenConferences);
        }

        public Task<bool> Create(Conference conference)
        {
            return _database.HashSetAsync(RedisKeys.OpenConferences, conference.ConferenceId, conference);
        }

        public Task<bool> Delete(string conferenceId)
        {
            return _database.HashDeleteAsync(RedisKeys.OpenConferences, conferenceId);
        }

        public Task<bool> Exists(string conferenceId)
        {
            return _database.HashExistsAsync(RedisKeys.OpenConferences, conferenceId);
        }
    }
}
