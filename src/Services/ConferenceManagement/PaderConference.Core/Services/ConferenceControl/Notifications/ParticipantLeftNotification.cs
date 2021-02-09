using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Notifications
{
    public record ParticipantLeftNotification
        (string ParticipantId, string ConferenceId, string ConnectionId) : INotification;
}
