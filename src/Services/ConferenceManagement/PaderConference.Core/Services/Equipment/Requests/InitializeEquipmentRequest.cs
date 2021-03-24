using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Equipment.Requests
{
    public record InitializeEquipmentRequest(Participant Participant, string ConnectionId, string Name,
        IReadOnlyList<EquipmentDevice> Devices) : IRequest;
}
