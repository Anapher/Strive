using MediatR;

namespace Strive.Core.Services.ConferenceControl.Notifications
{
    public record FinalizeConferenceCleanupNotification(string ConferenceId) : INotification;
}
