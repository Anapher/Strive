using MediatR;

namespace PaderConference.Core.Services.Equipment.Notifications
{
    public record EquipmentDisconnectedNotification(Participant Participant, string ConnectionId) : INotification;
}
