using System.Collections.Generic;
using MediatR;

namespace Strive.Core.Services.Synchronization.Requests
{
    public record FetchSubscribedParticipantsRequest
        (string ConferenceId, SynchronizedObjectId SyncObjId) : IRequest<IReadOnlyList<Participant>>;
}
