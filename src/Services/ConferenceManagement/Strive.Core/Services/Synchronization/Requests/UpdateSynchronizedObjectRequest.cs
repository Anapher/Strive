using MediatR;

namespace Strive.Core.Services.Synchronization.Requests
{
    public record UpdateSynchronizedObjectRequest
        (string ConferenceId, SynchronizedObjectId SynchronizedObjectId) : IRequest<Unit>;
}
