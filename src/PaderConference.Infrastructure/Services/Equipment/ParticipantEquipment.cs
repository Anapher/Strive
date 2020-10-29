using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Infrastructure.Services.Equipment.Data;
using PaderConference.Infrastructure.Services.Equipment.Dto;

namespace PaderConference.Infrastructure.Services.Equipment
{
    public class ParticipantEquipment
    {
        private readonly Dictionary<string, EquipmentConnection> _connectionIdToEquipment =
            new Dictionary<string, EquipmentConnection>();

        private readonly Dictionary<Guid, EquipmentConnection> _equipmentIdToEquipment =
            new Dictionary<Guid, EquipmentConnection>();

        private readonly object _lock = new object();

        public ValueTask OnEquipmentConnected(string connectionId)
        {
            lock (_lock)
            {
                if (_connectionIdToEquipment.ContainsKey(connectionId))
                    throw new InvalidOperationException("The equipment is already connected.");

                var connection = new EquipmentConnection(connectionId);
                _connectionIdToEquipment.Add(connectionId, connection);
                _equipmentIdToEquipment.Add(connection.EquipmentId, connection);
            }

            return new ValueTask();
        }

        public ValueTask OnEquipmentDisconnected(string connectionId)
        {
            lock (_lock)
            {
                if (_connectionIdToEquipment.Remove(connectionId, out var equipmentConnection))
                    _equipmentIdToEquipment.Remove(equipmentConnection.EquipmentId);
            }

            return new ValueTask();
        }

        public ValueTask RegisterEquipment(string connectionId, RegisterEquipmentRequestDto dto)
        {
            lock (_lock)
            {
                if (!_connectionIdToEquipment.TryGetValue(connectionId, out var equipmentConnection))
                    throw new InvalidOperationException("Equipment not connected");

                equipmentConnection.Name = dto.Name;
                equipmentConnection.Devices = dto.Devices;
            }

            return new ValueTask();
        }

        public ParticipantEquipmentStatusDto GetStatus()
        {
            lock (_lock)
            {
                return new ParticipantEquipmentStatusDto(_equipmentIdToEquipment.Values.Select(x =>
                        new ConnectedEquipmentDto {Name = x.Name, Devices = x.Devices, EquipmentId = x.EquipmentId})
                    .ToList());
            }
        }
    }
}
