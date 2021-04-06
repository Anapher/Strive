using System.Collections.Generic;
using Strive.Core.Services.Rooms.Notifications.Base;

namespace Strive.Core.Services.Rooms.Notifications
{
    public record RoomsCreatedNotification
        (string ConferenceId, IReadOnlyList<string> CreatedRoomIds) : RoomsChangedNotificationBase(ConferenceId);
}
