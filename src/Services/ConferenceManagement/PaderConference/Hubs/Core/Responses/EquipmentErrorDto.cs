using PaderConference.Core.Dto;

namespace PaderConference.Hubs.Core.Responses
{
    public record EquipmentErrorDto(string ConnectionId, Error Error);
}
