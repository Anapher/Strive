using PaderConference.Core.Dto;
using PaderConference.Core.Errors;

namespace PaderConference.Core.Services.Equipment
{
    public class EquipmentError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error InvalidToken =>
            BadRequest("Invalid token for equipment provided.", ServiceErrorCode.Equipment_InvalidToken);

        public static Error NotInitialized =>
            BadRequest("The equipment connection was not initialized.", ServiceErrorCode.Equipment_NotInitialized);

        public static Error ParticipantNotJoined =>
            BadRequest("The participant is not joined to the conference.",
                ServiceErrorCode.Equipment_ParticipantNotJoined);
    }
}
