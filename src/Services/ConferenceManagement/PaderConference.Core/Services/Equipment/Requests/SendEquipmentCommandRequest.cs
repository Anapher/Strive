using MediatR;
using PaderConference.Core.Services.Media.Dtos;

namespace PaderConference.Core.Services.Equipment.Requests
{
    public record SendEquipmentCommandRequest(Participant Participant, string ConnectionId, ProducerSource Source,
        string? DeviceId, EquipmentCommandType Action) : IRequest;
}
