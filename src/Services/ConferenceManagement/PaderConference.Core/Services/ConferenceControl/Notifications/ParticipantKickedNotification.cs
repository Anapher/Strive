using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Notifications
{
    public record ParticipantKickedNotification(string ParticipantId, string ConferenceId) : INotification;
}
