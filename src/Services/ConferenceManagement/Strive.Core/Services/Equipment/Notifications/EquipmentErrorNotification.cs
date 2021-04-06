using MediatR;
using Strive.Core.Dto;

namespace Strive.Core.Services.Equipment.Notifications
{
    public record EquipmentErrorNotification(Participant Participant, string ConnectionId, Error Error) : INotification;
}
