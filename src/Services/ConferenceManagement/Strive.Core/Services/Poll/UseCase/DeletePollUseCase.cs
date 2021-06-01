using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Poll.Utilities;
using Strive.Core.Services.Synchronization.Requests;

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
            var deletedAnswers = await _repository.DeletePollAnswers(conferenceId, pollId);

            await _mediator.Send(new UpdateParticipantSubscriptionsOfPollRequest(conferenceId, poll));
            await SceneUpdater.UpdateScene(_mediator, conferenceId, poll.RoomId);

            foreach (var participantId in deletedAnswers.Keys)
            {
                await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId,
                    SynchronizedPollAnswers.SyncObjId(participantId)));
            }

            return Unit.Value;
        }
    }
}
