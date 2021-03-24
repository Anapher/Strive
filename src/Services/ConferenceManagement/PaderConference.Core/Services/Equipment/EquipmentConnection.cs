using System.Collections.Generic;

namespace PaderConference.Core.Services.Equipment
{
    public record EquipmentConnection(string ConnectionId, string Name,
        IReadOnlyDictionary<string, EquipmentDevice> Devices, IReadOnlyDictionary<string, UseMediaStateInfo> Status);
}
