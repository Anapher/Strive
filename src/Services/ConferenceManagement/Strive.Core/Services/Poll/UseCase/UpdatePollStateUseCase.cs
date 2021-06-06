using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Poll.UseCase
{
    public class UpdatePollStateUseCase : IRequestHandler<UpdatePollStateRequest>
    {
        private readonly IPollRepository _repository;
        private readonly IMediator _mediator;

        public UpdatePollStateUseCase(IPollRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UpdatePollStateRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, pollId, pollState) = request;

            var previousState = await _repository.SetPollState(conferenceId, pollId, pollState);

            var poll = await _repository.GetPoll(conferenceId, pollId);
            if (poll == null)
            {
                await _repository.DeletePollAndState(conferenceId, pollId);
                throw PollError.PollNotFound.ToException();
            }

            await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId, SynchronizedPoll.SyncObjId(pollId)));

            if (request.State.ResultsPublished != previousState?.ResultsPublished)
            {
                await _mediator.Send(new UpdateParticipantSubscriptionsOfPollRequest(conferenceId, poll));
            }

            return Unit.Value;
        }
    }
}
