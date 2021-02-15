using MediatR;

namespace PaderConference.Core.Services.Synchronization.Requests
{
    public record UpdateSynchronizedObjectRequest
        (string ConferenceId, SynchronizedObjectId SynchronizedObjectId) : IRequest<Unit>;
}
