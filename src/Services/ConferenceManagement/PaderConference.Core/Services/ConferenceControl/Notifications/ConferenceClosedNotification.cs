namespace PaderConference.Core.Services.ConferenceControl.Notifications
{
    public record ConferenceClosedNotification(string ConferenceId) : ConferenceStateUpdatedNotification(ConferenceId);
}
