using System.Threading.Tasks;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Infrastructure.Redis.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class OpenConferenceRepository : IOpenConferenceRepository, IRedisRepo
    {
        private readonly IKeyValueDatabase _database;

        private const string PROPERTY_KEY = "IsOpen";

        public OpenConferenceRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async Task<bool> Create(string conferenceId)
        {
            var key = GetKey(conferenceId);
            return await _database.GetSetAsync(key, bool.TrueString) == null;
        }

        public async Task<bool> Delete(string conferenceId)
        {
            var key = GetKey(conferenceId);
            return await _database.KeyDeleteAsync(key);
        }

        public async Task<bool> IsOpen(string conferenceId)
        {
            var key = GetKey(conferenceId);
            return await _database.GetAsync(key) == bool.TrueString;
        }

        private string GetKey(string conferenceId)
        {
            return RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
