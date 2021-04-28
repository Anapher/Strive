using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Synchronization.UseCases
{
    public class ClearCacheRequestHandler : IRequestHandler<ClearCacheRequest>
    {
        public Task<Unit> Handle(ClearCacheRequest request, CancellationToken cancellationToken)
        {
            // no cache by default
            return Task.FromResult(Unit.Value);
        }
    }
}
