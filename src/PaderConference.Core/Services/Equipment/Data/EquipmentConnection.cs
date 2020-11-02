using System;
using System.Collections.Generic;

namespace PaderConference.Core.Services.Equipment.Data
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

        public Dictionary<string, UseMediaStateInfo>? Status { get; set; }
    }
}
