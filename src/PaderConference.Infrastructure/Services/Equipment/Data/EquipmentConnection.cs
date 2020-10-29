using System;
using System.Collections.Generic;

namespace PaderConference.Infrastructure.Services.Equipment.Data
{
    public class EquipmentConnection
    {
        public EquipmentConnection(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public string ConnectionId { get; }

        public Guid EquipmentId { get; } = Guid.NewGuid();

        public string? Name { get; set; }

        public List<EquipmentDeviceInfo>? Devices { get; set; }
    }
}
