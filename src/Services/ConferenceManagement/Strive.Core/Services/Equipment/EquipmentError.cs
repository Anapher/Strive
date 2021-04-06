using Strive.Core.Dto;
using Strive.Core.Errors;

namespace Strive.Core.Services.Equipment
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
