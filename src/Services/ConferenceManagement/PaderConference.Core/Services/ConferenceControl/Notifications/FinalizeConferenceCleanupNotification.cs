using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Notifications
{
    public record FinalizeConferenceCleanupNotification(string ConferenceId) : INotification;
}
