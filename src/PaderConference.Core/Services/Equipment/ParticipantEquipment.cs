using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Services.Equipment.Data;
using PaderConference.Core.Services.Equipment.Dto;

namespace PaderConference.Core.Services.Equipment
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

        public EquipmentConnection? GetConnection(Guid equipmentId)
        {
            lock (_lock)
            {
                _equipmentIdToEquipment.TryGetValue(equipmentId, out var connection);
                return connection;
            }
        }

        public void UpdateStatus(string connectionId, Dictionary<string, UseMediaStateInfo> status)
        {
            lock (_lock)
            {
                if (!_connectionIdToEquipment.TryGetValue(connectionId, out var connection))
                    throw new InvalidOperationException("Equipment not found");

                connection.Status = status;
            }
        }

        public List<ConnectedEquipmentDto> GetStatus()
        {
            lock (_lock)
            {
                var connectedEquipment = _equipmentIdToEquipment.Values.Select(x =>
                    new ConnectedEquipmentDto
                    {
                        Name = x.Name, Devices = x.Devices, EquipmentId = x.EquipmentId, Status = x.Status,
                    }).ToList();

                return connectedEquipment;
            }
        }
    }
}
