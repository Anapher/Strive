using PaderConference.Core.Dto;
using PaderConference.Core.Errors;

namespace PaderConference.Core.Services.BreakoutRoom
{
    public class BreakoutRoomError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error AlreadyOpen =>
            BadRequest("Cannot open breakout rooms as they are already open. Please close them first.",
                ServiceErrorCode.BreakoutRoom_AlreadyOpen);

        public static Error NotOpen =>
            new BadRequestError<ServiceErrorCode>("Breakout rooms are not open.",
                ServiceErrorCode.BreakoutRoom_NotOpen);

        public static Error AssigningParticipantsFailed =>
            InternalServerError("Assigning participants failed.",
                ServiceErrorCode.BreakoutRoom_AssigningParticipantsFailed);
    }
}
