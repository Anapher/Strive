using Strive.Core.Services.Equipment;
using Strive.Core.Services.Media.Dtos;

namespace Strive.Hubs.Equipment.Responses
{
    public record EquipmentCommandDto(ProducerSource Source, string? DeviceId, EquipmentCommandType Action);
}
