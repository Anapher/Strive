using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Equipment.Requests
{
    public record UpdateStatusRequest(Participant Participant, string ConnectionId,
        IReadOnlyDictionary<string, UseMediaStateInfo> Status) : IRequest<Unit>;
}
