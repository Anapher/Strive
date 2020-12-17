using PaderConference.Core.Dto;

namespace PaderConference.Core.Services.ConferenceControl
{
    public static class ConferenceError
    {
        public static Error NotFound =
            new ServiceError("The conference was not found.", ServiceErrorCode.Conference_NotFound);

        public static Error NotOpen =
            new ServiceError("The conference is not open.", ServiceErrorCode.Conference_NotOpen);

        public static Error PermissionDeniedToOpenOrClose =
            new ServiceError("Permission denied to open or close the conference.",
                ServiceErrorCode.Conference_PermissionDeniedToOpenOrClose);

        public static Error ParticipantNotRegistered =
            new ServiceError("The participant was not registered with the current conference.",
                ServiceErrorCode.Conference_ParticipantNotRegistered);

        public static Error InternalServiceError = new ServiceError("An internal service error occurred..",
            ServiceErrorCode.Conference_InternalServiceError);

        public static Error UnexpectedError(string message)
        {
            return new ServiceError(message, ServiceErrorCode.Conference_UnexpectedError);
        }
    }
}