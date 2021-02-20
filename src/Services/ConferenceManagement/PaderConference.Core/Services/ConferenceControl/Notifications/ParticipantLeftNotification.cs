using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Notifications
{
    public record ParticipantLeftNotification (Participant Participant, string ConnectionId) : INotification;
}
