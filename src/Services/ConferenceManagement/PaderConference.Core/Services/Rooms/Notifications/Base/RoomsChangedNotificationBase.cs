using MediatR;

namespace PaderConference.Core.Services.Rooms.Notifications.Base
{
    public abstract record RoomsChangedNotificationBase(string ConferenceId) : INotification;
}
