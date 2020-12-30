// ReSharper disable InconsistentNaming

namespace PaderConference.Core.Services
{
    public enum ServiceErrorCode
    {
        // 0-1000 reserved for UI

        // 1001- 2000 reserved for SFU

        // Common
        ParticipantNotFound,
        PermissionDenied,


        // Conference
        Conference_NotFound = 1000000,
        Conference_UnexpectedError,
        Conference_NotOpen,
        Conference_ParticipantNotRegistered,
        Conference_InternalServiceError,
        Conference_ParticipantConnectionNotFound,

        // Chat
        Chat_InvalidMode = 1000003,

        // Rooms
        Rooms_NotFound = 1000203,

        // Permissions
        Permissions_PermissionKeyNotFound = 1000300,
        Permissions_InvalidPermissionValueType = 1000302,

        // Equipment
        Equipment_NotFound = 1000400,

        // Scenes
        Scenes_RoomNotFound = 1000501,

        // Breakout Rooms
        BreakoutRoom_AlreadyOpen = 1000600,
        BreakoutRoom_NotOpen = 1000602,
        BreakoutRoom_AssigningParticipantsFailed,
    }
}
