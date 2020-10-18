using PaderConference.Core.Dto;

namespace PaderConference.Infrastructure.Services.Rooms
{
    public static class RoomsError
    {
        public static Error SwitchRoomFailed =>
            new ServiceError("Switching the room failed.", ServiceErrorCode.Rooms_SwitchRoomFailed);

        public static Error PermissionToSwitchRoomDenied =>
            new ServiceError("Permissions to switch room denied.", ServiceErrorCode.Rooms_SwitchRoomFailed);

        public static Error PermissionToCreateRoomDenied =>
            new ServiceError("Permissions to create room denied.", ServiceErrorCode.Rooms_CreateDenied);

        public static Error PermissionToRemoveRoomDenied =>
            new ServiceError("Permissions to remove room denied.", ServiceErrorCode.Rooms_RemoveDenied);
    }
}