using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Synchronization.Gateways;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Synchronization.UseCases
{
    public class FetchSynchronizedObjectUseCase : IRequestHandler<FetchSynchronizedObjectRequest, object>
    {
        private readonly ISynchronizedObjectRepository _repository;
        private readonly IEnumerable<ISynchronizedObjectProvider> _providers;

        public FetchSynchronizedObjectUseCase(ISynchronizedObjectRepository repository,
            IEnumerable<ISynchronizedObjectProvider> providers)
        {
            _repository = repository;
            _providers = providers;
        }

        public async Task<object> Handle(FetchSynchronizedObjectRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, syncObjId) = request;

            var provider = _providers.First(x => x.Id == syncObjId.Id);

            var currentStoredValue = await _repository.Get(conferenceId, syncObjId.ToString(), provider.Type);
            if (currentStoredValue != null) return currentStoredValue;

            var currentValue = await provider.FetchValue(conferenceId, syncObjId);
            await _repository.Create(conferenceId, syncObjId.ToString(), currentValue, provider.Type);

            return currentValue;
        }
    }
}
