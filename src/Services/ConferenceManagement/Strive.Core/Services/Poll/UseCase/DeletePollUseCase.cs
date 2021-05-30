using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;

namespace Strive.Core.Services.Poll.UseCase
{
    public class DeletePollUseCase : IRequestHandler<DeletePollRequest>
    {
        private readonly IPollRepository _repository;
        private readonly IMediator _mediator;

        public DeletePollUseCase(IPollRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(DeletePollRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, pollId) = request;

            var poll = await _repository.GetPoll(conferenceId, pollId);
            if (poll == null) return Unit.Value;

            await _repository.DeletePollAndState(conferenceId, pollId);
            await _repository.DeletePollAnswers(conferenceId, pollId);

            await _mediator.Send(new UpdateParticipantSubscriptionsOfPollRequest(conferenceId, poll));
            return Unit.Value;
        }
    }
}
