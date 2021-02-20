using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Notifications
{
    public record ParticipantInitializedNotification(Participant Participant) : INotification;
}
