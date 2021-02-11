using System.Collections.Generic;
using PaderConference.Core.Services.Rooms.Notifications.Base;

namespace PaderConference.Core.Services.Rooms.Notifications
{
    public record RoomsCreatedNotification
        (string ConferenceId, IReadOnlyList<string> CreatedRoomIds) : RoomsChangedNotificationBase(ConferenceId);
}
