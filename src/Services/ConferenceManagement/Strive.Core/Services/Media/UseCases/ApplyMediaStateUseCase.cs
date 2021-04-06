using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Media.Gateways;
using Strive.Core.Services.Media.Requests;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Media.UseCases
{
    public class ApplyMediaStateUseCase : IRequestHandler<ApplyMediaStateRequest>
    {
        private readonly IMediaStateRepository _repository;
        private readonly IMediator _mediator;

        public ApplyMediaStateUseCase(IMediaStateRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ApplyMediaStateRequest request, CancellationToken cancellationToken)
        {
            await _repository.Set(request.ConferenceId, request.Payload);

            await _mediator.Send(new UpdateSynchronizedObjectRequest(request.ConferenceId,
                SynchronizedMediaState.SyncObjId));

            return Unit.Value;
        }
    }
}
