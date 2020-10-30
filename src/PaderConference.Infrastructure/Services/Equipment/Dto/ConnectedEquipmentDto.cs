using System;
using System.Collections.Generic;
using PaderConference.Infrastructure.Services.Equipment.Data;

namespace PaderConference.Infrastructure.Services.Equipment.Dto
{
    public class ConnectedEquipmentDto
    {
        public Guid EquipmentId { get; set; }

        public string? Name { get; set; }

        public List<EquipmentDeviceInfo>? Devices { get; set; }

        // temporary, normally it's ProducerSource -> UseMediaStateInfo, but the JSON serializer only supports strings
        public Dictionary<string, UseMediaStateInfo>? Status { get; set; }
    }
}
