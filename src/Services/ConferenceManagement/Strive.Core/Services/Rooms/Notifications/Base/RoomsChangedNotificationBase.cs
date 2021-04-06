using MediatR;

namespace Strive.Core.Services.Rooms.Notifications.Base
{
    public abstract record RoomsChangedNotificationBase(string ConferenceId) : INotification;
}
