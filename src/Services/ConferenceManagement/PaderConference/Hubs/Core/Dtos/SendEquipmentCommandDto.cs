using PaderConference.Core.Services.Equipment;
using PaderConference.Core.Services.Media.Dtos;

namespace PaderConference.Hubs.Core.Dtos
{
    public record SendEquipmentCommandDto(string ConnectionId, ProducerSource Source, string? DeviceId,
        EquipmentCommandType Action);
}
