using System.Collections.Generic;
using MediatR;

namespace Strive.Core.Services.Synchronization.Requests
{
    public record FetchParticipantSubscriptionsRequest
        (Participant Participant) : IRequest<IReadOnlyList<SynchronizedObjectId>>;
}
