using MediatR;
using PaderConference.Core.Services.Synchronization.UpdateStrategy;

namespace PaderConference.Core.Services.Synchronization.Requests
{
    public record UpdateSynchronizedObjectRequest<T>
        (IValueUpdate<T> ValueUpdate, SynchronizedObjectMetadata Metadata) : IRequest<T> where T : notnull;
}
