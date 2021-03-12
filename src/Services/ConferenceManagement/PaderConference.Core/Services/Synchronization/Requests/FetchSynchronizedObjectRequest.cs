using MediatR;

namespace PaderConference.Core.Services.Synchronization.Requests
{
    public record FetchSynchronizedObjectRequest
        (string ConferenceId, SynchronizedObjectId SyncObjId) : IRequest<object>;
}
