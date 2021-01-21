using System.Collections.Generic;
using PaderConference.Core.Services.Equipment.Data;

namespace PaderConference.Core.Services.Equipment.Dto
{
    public class RegisterEquipmentRequestDto
    {
        public string? Name { get; set; }

        public List<EquipmentDeviceInfo>? Devices { get; set; }
    }
}
