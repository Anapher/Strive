using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Synchronization.Requests
{
    public record FetchSubscribedParticipantsRequest
        (string ConferenceId, SynchronizedObjectId SyncObjId) : IRequest<IReadOnlyList<Participant>>;
}
