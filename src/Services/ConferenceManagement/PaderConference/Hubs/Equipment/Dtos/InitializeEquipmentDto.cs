using System.Collections.Generic;
using PaderConference.Core.Services.Equipment;

namespace PaderConference.Hubs.Equipment.Dtos
{
    public record InitializeEquipmentDto(string Name, IReadOnlyList<EquipmentDevice> Devices);
}
