using Strive.Core.Dto;
using Strive.Core.Errors;

namespace Strive.Core.Services.Poll
{
    public class PollError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error PollNotFound => NotFound("The poll was not found.", ServiceErrorCode.Poll_NotFound);

        public static Error PollClosed => NotFound("The poll was closed.", ServiceErrorCode.Poll_Closed);

        public static Error AnswerValidationFailed(string message)
        {
            return NotFound(message, ServiceErrorCode.AnswerValidationFailed);
        }

        public static Error AnswerCannotBeChanged =>
            NotFound("You already submitted an answer to this poll and it cannot be changed.",
                ServiceErrorCode.Poll_AnswerCannotBeChanged);
    }
}
