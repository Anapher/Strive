using System.Collections.Generic;
using PaderConference.Core.Services.Rooms.Notifications.Base;

namespace PaderConference.Core.Services.Rooms.Notifications
{
    public record RoomsRemovedNotification
        (string ConferenceId, IReadOnlyList<string> RemovedRoomIds) : RoomsChangedNotificationBase(ConferenceId);
}
