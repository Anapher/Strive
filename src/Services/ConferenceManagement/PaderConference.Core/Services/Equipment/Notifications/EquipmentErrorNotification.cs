using MediatR;
using PaderConference.Core.Dto;

namespace PaderConference.Core.Services.Equipment.Notifications
{
    public record EquipmentErrorNotification(Participant Participant, string ConnectionId, Error Error) : INotification;
}
