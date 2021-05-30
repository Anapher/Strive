using System.Collections.Generic;
using MediatR;

namespace Strive.Core.Services.Poll.Requests
{
    public record FetchParticipantPollsRequest(Participant Participant) : IRequest<IReadOnlyList<Poll>>;
}
