using MediatR;

namespace Strive.Core.Services.Equipment.Notifications
{
    public record EquipmentDisconnectedNotification(Participant Participant, string ConnectionId) : INotification;
}
