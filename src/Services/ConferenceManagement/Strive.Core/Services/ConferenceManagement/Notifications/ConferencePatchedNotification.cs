using Strive.Core.Services.ConferenceControl.Notifications;

namespace Strive.Core.Services.ConferenceManagement.Notifications
{
    public record ConferencePatchedNotification(string ConferenceId) : ConferenceStateUpdatedNotification(ConferenceId);
}
