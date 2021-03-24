using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Equipment.Gateways
{
    public interface IEquipmentConnectionRepository
    {
        ValueTask SetConnection(Participant participant, EquipmentConnection connection);

        ValueTask<IReadOnlyDictionary<string, EquipmentConnection>> GetConnections(Participant participant);

        ValueTask<EquipmentConnection?> GetConnection(Participant participant, string connectionId);

        ValueTask RemoveConnection(Participant participant, string connectionId);

        ValueTask RemoveAllConnections(Participant participant);
    }
}
