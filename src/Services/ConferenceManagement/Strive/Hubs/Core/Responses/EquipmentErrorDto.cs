using Strive.Core.Dto;

namespace Strive.Hubs.Core.Responses
{
    public record EquipmentErrorDto(string ConnectionId, Error Error);
}
