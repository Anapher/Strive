using System.Collections.Generic;
using PaderConference.Infrastructure.Services.Equipment.Data;

namespace PaderConference.Infrastructure.Services.Equipment.Dto
{
    public class RegisterEquipmentRequestDto
    {
        public string? Name { get; set; }

        public List<EquipmentDeviceInfo>? Devices { get; set; }
    }
}
