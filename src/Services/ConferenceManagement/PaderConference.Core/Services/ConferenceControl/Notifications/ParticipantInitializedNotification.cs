using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Notifications
{
    public record ParticipantInitializedNotification(string ParticipantId, string ConferenceId) : INotification;
}
