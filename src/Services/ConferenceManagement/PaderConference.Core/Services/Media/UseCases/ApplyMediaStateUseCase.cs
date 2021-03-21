using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Media.Gateways;
using PaderConference.Core.Services.Media.Requests;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Media.UseCases
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
                SynchronizedMediaStateProvider.SyncObjId));

            return Unit.Value;
        }
    }
}
