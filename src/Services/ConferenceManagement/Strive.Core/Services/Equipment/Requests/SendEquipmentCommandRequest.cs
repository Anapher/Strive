using MediatR;
using Strive.Core.Services.Media.Dtos;

namespace Strive.Core.Services.Equipment.Requests
{
    public record SendEquipmentCommandRequest(Participant Participant, string ConnectionId, ProducerSource Source,
        string? DeviceId, EquipmentCommandType Action) : IRequest;
}
