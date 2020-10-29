using System.Collections.Generic;

namespace PaderConference.Infrastructure.Services.Equipment.Dto
{
    public class ParticipantEquipmentStatusDto
    {
        public ParticipantEquipmentStatusDto(List<ConnectedEquipmentDto> connectedEquipment)
        {
            ConnectedEquipment = connectedEquipment;
        }

        public List<ConnectedEquipmentDto> ConnectedEquipment { get; }
    }
}
