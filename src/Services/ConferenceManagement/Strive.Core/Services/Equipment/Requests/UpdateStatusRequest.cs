using System.Collections.Generic;
using MediatR;
using Strive.Core.Services.Media.Dtos;

namespace Strive.Core.Services.Equipment.Requests
{
    public record UpdateStatusRequest(Participant Participant, string ConnectionId,
        IReadOnlyDictionary<ProducerSource, UseMediaStateInfo> Status) : IRequest<Unit>;
}
