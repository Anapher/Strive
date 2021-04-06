using MediatR;

namespace Strive.Core.Services.ConferenceControl.Notifications
{
    public record ParticipantInitializedNotification(Participant Participant) : INotification;
}
