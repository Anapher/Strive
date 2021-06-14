using Strive.Core.Dto;
using Strive.Core.Errors;
using Strive.Core.Services;

namespace Strive.Core
{
    public class ConferenceError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error ConferenceNotFound =>
            NotFound("The conference was not found.", ServiceErrorCode.Conference_NotFound);

        public static Error ConferenceNotOpen =>
            Conflict("The conference is not open.", ServiceErrorCode.Conference_NotOpen);

        public static Error RoomNotFound =>
            NotFound("The room was not found.", ServiceErrorCode.Conference_RoomNotFound);

        public static Error UnexpectedError(string message)
        {
            return BadRequest(message, ServiceErrorCode.Conference_UnexpectedError);
        }
    }
}
