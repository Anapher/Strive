using PaderConference.Core.Dto;

namespace PaderConference.Core.Services
{
    public class CommonError
    {
        public static Error ParticipantNotFound =>
            new ServiceError("The participant was not found.", ServiceErrorCode.ParticipantNotFound);
    }
}
