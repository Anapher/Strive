using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Notifications
{
    public record ParticipantJoinedNotification(string ParticipantId, string ConferenceId) : INotification;
}
