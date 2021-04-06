using Strive.Core.Services.Equipment;
using Strive.Core.Services.Media.Dtos;

namespace Strive.Hubs.Core.Dtos
{
    public record SendEquipmentCommandDto(string ConnectionId, ProducerSource Source, string? DeviceId,
        EquipmentCommandType Action);
}
