using System.Threading.Tasks;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class OpenConferenceRepository : IOpenConferenceRepository, IKeyValueRepo
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
            return DatabaseKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
