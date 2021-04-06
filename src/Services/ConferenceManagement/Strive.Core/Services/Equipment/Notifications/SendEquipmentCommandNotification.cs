using MediatR;
using Strive.Core.Services.Media.Dtos;

namespace Strive.Core.Services.Equipment.Notifications
{
    public record SendEquipmentCommandNotification(Participant Participant, string ConnectionId, ProducerSource Source,
        string? DeviceId, EquipmentCommandType Action) : INotification;
}
