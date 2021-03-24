using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Equipment;
using PaderConference.Core.Services.Equipment.Gateways;
using PaderConference.Infrastructure.KeyValue.Abstractions;
using PaderConference.Infrastructure.KeyValue.Extensions;

namespace PaderConference.Infrastructure.KeyValue.Repos
{
    public class EquipmentConnectionRepository : IEquipmentConnectionRepository, IKeyValueRepo
    {
        private const string PROPERTY_KEY = "equipmentConnection";

        private readonly IKeyValueDatabase _database;

        public EquipmentConnectionRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask SetConnection(Participant participant, EquipmentConnection connection)
        {
            var key = GetKey(participant);
            await _database.HashSetAsync(key, connection.ConnectionId, connection);
        }

        public async ValueTask<IReadOnlyDictionary<string, EquipmentConnection>> GetConnections(Participant participant)
        {
            var key = GetKey(participant);
            return (await _database.HashGetAllAsync<EquipmentConnection>(key))!;
        }

        public ValueTask<EquipmentConnection?> GetConnection(Participant participant, string connectionId)
        {
            var key = GetKey(participant);
            return _database.HashGetAsync<EquipmentConnection>(key, connectionId);
        }

        public async ValueTask RemoveConnection(Participant participant, string connectionId)
        {
            var key = GetKey(participant);
            await _database.HashDeleteAsync(key, connectionId);
        }

        public async ValueTask RemoveAllConnections(Participant participant)
        {
            var key = GetKey(participant);
            await _database.KeyDeleteAsync(key);
        }

        private static string GetKey(Participant participant)
        {
            return DatabaseKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(participant.ConferenceId)
                .ForSecondary(participant.Id).ToString();
        }
    }
}
