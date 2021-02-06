namespace PaderConference.Core.Services.ConferenceControl.Notifications
{
    public record ConferenceOpenedNotification(string ConferenceId) : ConferenceStateUpdatedNotification(ConferenceId);
}
