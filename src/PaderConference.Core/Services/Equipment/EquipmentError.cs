using PaderConference.Core.Dto;

namespace PaderConference.Core.Services.Equipment
{
    public class EquipmentError
    {
        public static Error NotFound => new ServiceError("Equipment not found.", ServiceErrorCode.Equipment_NotFound);
    }
}
