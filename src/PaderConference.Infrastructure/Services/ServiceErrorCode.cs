// ReSharper disable InconsistentNaming

namespace PaderConference.Infrastructure.Services
{
    public enum ServiceErrorCode
    {
        // Conference
        Conference_NotFound = 1000000,
        Conference_UnexpectedError,

        // Chat
        Chat_EmptyMessage = 1000100,
        Chat_PermissionDenied_SendMessage = 1000101,
        Chat_PermissionDenied_SendAnonymousMessage = 1000102,
        Chat_PermissionDenied_SendPrivateMessage = 1000102,
        Chat_InvalidFilter = 1000003,

        // Rooms
        Rooms_SwitchRoomFailed = 1000200,
        Rooms_SwitchRoomDenied = 1000201
    }
}