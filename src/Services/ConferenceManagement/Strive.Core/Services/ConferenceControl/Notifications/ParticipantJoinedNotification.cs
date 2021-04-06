using MediatR;

namespace Strive.Core.Services.ConferenceControl.Notifications
{
    public record ParticipantJoinedNotification(Participant Participant, ParticipantMetadata Meta) : INotification;
}
