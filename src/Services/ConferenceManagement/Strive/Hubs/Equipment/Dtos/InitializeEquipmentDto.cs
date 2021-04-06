using System.Collections.Generic;
using Strive.Core.Services.Equipment;

namespace Strive.Hubs.Equipment.Dtos
{
    public record InitializeEquipmentDto(string Name, IReadOnlyList<EquipmentDevice> Devices);
}
