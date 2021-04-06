using System.Collections.Generic;
using Strive.Core.Services.Rooms.Notifications.Base;

namespace Strive.Core.Services.Rooms.Notifications
{
    public record RoomsRemovedNotification
        (string ConferenceId, IReadOnlyList<string> RemovedRoomIds) : RoomsChangedNotificationBase(ConferenceId);
}
