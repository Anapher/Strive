using PaderConference.Core.Dto;

namespace PaderConference.Core.Services.BreakoutRoom
{
    public static class BreakoutRoomError
    {
        public static Error AlreadyOpen =>
            new ServiceError("Cannot open breakout rooms as they are already open. Please close them first.",
                ServiceErrorCode.BreakoutRoom_AlreadyOpen);

        public static Error NotOpen =>
            new ServiceError("Breakout rooms are not open.", ServiceErrorCode.BreakoutRoom_NotOpen);

        public static Error CannotAssignParticipants =>
            new ServiceError("Cannot assign participants to rooms.", ServiceErrorCode.BreakoutRoom_CannotAssign);

        public static Error AssigningParticipantsFailed =>
            new ServiceError("Assigning participants failed.",
                ServiceErrorCode.BreakoutRoom_AssigningParticipantsFailed);

        public static Error AmountMustBePositiveNumber =>
            new ServiceError("Please submit an amount that is equal or greater than one.",
                ServiceErrorCode.BreakoutRoom_AmountMustBePositiveNumber);
    }
}
