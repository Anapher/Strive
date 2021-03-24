using PaderConference.Core.Services.Equipment;
using PaderConference.Core.Services.Media.Dtos;

namespace PaderConference.Hubs.Equipment.Responses
{
    public record EquipmentCommandDto(ProducerSource Source, string? DeviceId, EquipmentCommandType Action);
}
