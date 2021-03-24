using System.Threading.Tasks;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Equipment.Gateways;
using PaderConference.Infrastructure.KeyValue.Abstractions;

namespace PaderConference.Infrastructure.KeyValue.Repos
{
    public class EquipmentTokenRepository : IEquipmentTokenRepository, IKeyValueRepo
    {
        private const string PROPERTY_KEY = "equipmentToken";

        private readonly IKeyValueDatabase _database;

        public EquipmentTokenRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask RemoveAllDataOfConference(string conferenceId)
        {
            var key = GetKey(conferenceId);
            await _database.KeyDeleteAsync(key);
        }

        public ValueTask Set(Participant participant, string token)
        {
            var key = GetKey(participant.ConferenceId);
            return _database.HashSetAsync(key, participant.Id, token);
        }

        public ValueTask<string?> Get(Participant participant)
        {
            var key = GetKey(participant.ConferenceId);
            return _database.HashGetAsync(key, participant.Id);
        }

        private static string GetKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
