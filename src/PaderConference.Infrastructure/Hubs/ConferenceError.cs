using PaderConference.Core.Dto;
using PaderConference.Infrastructure.Services;

namespace PaderConference.Infrastructure.Hubs
{
    public static class ConferenceError
    {
        public static Error NotFound =
            new ServiceError("The conference was not found.", ServiceErrorCode.Conference_NotFound);

        public static Error UnexpectedError(string message)
        {
            return new ServiceError(message, ServiceErrorCode.Conference_UnexpectedError);
        }
    }
}