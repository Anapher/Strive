using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Notifications
{
    public record ParticipantJoinedNotification(Participant Participant, ParticipantMetadata Meta) : INotification;
}
