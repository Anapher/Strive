using System.Collections.Generic;
using Strive.Core.Services.Media.Dtos;

namespace Strive.Core.Services.Equipment
{
    public record EquipmentConnection(string ConnectionId, string Name, IReadOnlyList<EquipmentDevice> Devices,
        IReadOnlyDictionary<ProducerSource, UseMediaStateInfo> Status);
}
