using System.Collections.Generic;

namespace PaderConference.Core.Services.Equipment
{
    public record SynchronizedEquipment(IReadOnlyDictionary<string, EquipmentConnection> Connections);
}
