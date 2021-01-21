using PaderConference.Core.Dto;
using PaderConference.Core.Errors;

namespace PaderConference.Core.Services.Equipment
{
    public class EquipmentError : ErrorsProvider<ServiceErrorCode>
    {
        public static Error EquipmentNotFound => NotFound("Equipment not found.", ServiceErrorCode.Equipment_NotFound);
    }
}
