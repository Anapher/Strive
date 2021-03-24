using MediatR;
using PaderConference.Core.Services.Media.Dtos;

namespace PaderConference.Core.Services.Equipment.Notifications
{
    public record SendEquipmentCommandNotification(Participant Participant, string ConnectionId, ProducerSource Source,
        string? DeviceId, EquipmentCommandType Action) : INotification;
}
