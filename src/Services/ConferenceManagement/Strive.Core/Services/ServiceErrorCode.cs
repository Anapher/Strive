// ReSharper disable InconsistentNaming

namespace Strive.Core.Services
{
    public enum ServiceErrorCode
    {
        // 0-1000 reserved for UI

        // 1001- 2000 reserved for SFU

        // Common
        PermissionDenied,


        // Conference
        Conference_NotFound,
        Conference_UnexpectedError,
        Conference_NotOpen,
        Conference_InternalServiceError,
        Conference_RoomNotFound,

        // Chat
        Chat_InvalidChannel,
        Chat_PrivateMessagesDisabled,

        // Permissions
        Permissions_PermissionKeyNotFound,
        Permissions_InvalidPermissionValueType,

        // Equipment
        Equipment_InvalidToken,
        Equipment_NotInitialized,
        Equipment_ParticipantNotJoined,

        // Scenes
        Scenes_HasSpeaker,

        // Breakout Rooms
        BreakoutRoom_AlreadyOpen,
        BreakoutRoom_NotOpen,
        BreakoutRoom_AssigningParticipantsFailed,

        // Poll
        Poll_NotFound,
        AnswerValidationFailed,
        Poll_AnswerCannotBeChanged,
        Poll_Closed,

        // Whiteboard
        Whiteboard_NotFound,
        Whiteboard_UndoNotAvailable,
        Whiteboard_RedoNotAvailable,
        Whiteboard_WhiteboardActionHadNoEffect,
    }
}
