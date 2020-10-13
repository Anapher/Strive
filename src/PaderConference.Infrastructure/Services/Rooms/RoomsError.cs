using PaderConference.Infrastructure.Services.Chat;

namespace PaderConference.Infrastructure.Services.Rooms
{
    public class RoomsError : ServiceError
    {
        public RoomsError(string message, ServiceErrorCode code) : base(message, code)
        {
        }

        public static ChatError SwitchRoomFailed =>
            new ChatError("Switching the room failed.", ServiceErrorCode.Rooms_SwitchRoomFailed);

        public static ChatError PermissionToSwitchRoomDenied =>
            new ChatError("Permissions to switch room denied.", ServiceErrorCode.Rooms_SwitchRoomFailed);
    }
}