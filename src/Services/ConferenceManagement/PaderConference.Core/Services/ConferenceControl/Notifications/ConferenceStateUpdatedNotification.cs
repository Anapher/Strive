using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Notifications
{
    public abstract record ConferenceStateUpdatedNotification(string ConferenceId) : INotification;
}
