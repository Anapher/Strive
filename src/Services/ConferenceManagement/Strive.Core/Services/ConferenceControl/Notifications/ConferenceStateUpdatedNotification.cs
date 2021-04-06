using MediatR;

namespace Strive.Core.Services.ConferenceControl.Notifications
{
    public abstract record ConferenceStateUpdatedNotification(string ConferenceId) : INotification;
}
