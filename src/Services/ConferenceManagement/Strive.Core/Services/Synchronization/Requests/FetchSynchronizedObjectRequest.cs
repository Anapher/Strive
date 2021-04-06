using MediatR;

namespace Strive.Core.Services.Synchronization.Requests
{
    public record FetchSynchronizedObjectRequest
        (string ConferenceId, SynchronizedObjectId SyncObjId) : IRequest<object>;
}
