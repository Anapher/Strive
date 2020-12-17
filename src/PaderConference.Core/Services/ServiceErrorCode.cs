// ReSharper disable InconsistentNaming

namespace PaderConference.Core.Services
{
    public enum ServiceErrorCode
    {
        // Conference
        Conference_NotFound = 1000000,
        Conference_UnexpectedError,
        Conference_NotOpen,
        Conference_PermissionDeniedToOpenOrClose,

        // Chat
        Chat_EmptyMessage = 1000100,
        Chat_PermissionDenied_SendMessage = 1000101,
        Chat_PermissionDenied_SendAnonymousMessage = 1000102,
        Chat_PermissionDenied_SendPrivateMessage = 1000102,
        Chat_InvalidMode = 1000003,
        Chat_InvalidParticipant = 1000004,

        // Rooms
        Rooms_SwitchRoomFailed = 1000200,
        Rooms_SwitchRoomDenied = 1000201,
        Rooms_CreateDenied = 1000202,
        Rooms_RemoveDenied = 1000203,

        // Permissions
        Permissions_PermissionKeyNotFound = 1000300,
        Permissions_DeniedGiveTemporaryPermission = 1000301,
        Permissions_InvalidPermissionValueType = 1000302,
        Permissions_ParticipantNotFound = 1000303,
        Permissions_DeniedFetchingParticipantsPermissions = 1000304,

        // Equipment
        Equipment_NotFound = 1000400,

        // Scenes
        Scenes_PermissionDenied_Change = 1000500,
        Scenes_RoomNotFound = 1000501,
        Scenes_SceneMustNotBeNull = 1000502,

        // Breakout Rooms
        BreakoutRoom_AlreadyOpen = 1000600,
        BreakoutRoom_CannotAssign = 1000601,
        BreakoutRoom_NotOpen = 1000602,
        BreakoutRoom_AssigningParticipantsFailed = 1000603,
        BreakoutRoom_AmountMustBePositiveNumber = 1000604,
    }
}
