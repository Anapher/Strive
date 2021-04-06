using MediatR;

namespace Strive.Core.Services.ConferenceControl.Notifications
{
    public record ParticipantLeftNotification (Participant Participant, string ConnectionId) : INotification;
}
