using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Poll.Utilities;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Poll.UseCase
{
    public class SubmitAnswerUseCase : IRequestHandler<SubmitAnswerRequest>
    {
        private readonly IPollRepository _repository;
        private readonly IComponentContext _context;
        private readonly IMediator _mediator;

        public SubmitAnswerUseCase(IPollRepository repository, IComponentContext context, IMediator mediator)
        {
            _repository = repository;
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(SubmitAnswerRequest request, CancellationToken cancellationToken)
        {
            var (participant, pollId, answer) = request;

            var poll = await _repository.GetPoll(participant.ConferenceId, pollId);
            if (poll == null)
                throw PollError.PollNotFound.ToException();

            var state = await _repository.GetPollState(participant.ConferenceId, pollId);
            if (state?.IsOpen != true)
                throw PollError.PollClosed.ToException();

            var wrapper = new PollAnswerValidatorWrapper(_context);
            if (!wrapper.Validate(poll.Instruction, answer))
                throw PollError.InvalidAnswer.ToException();

            if (poll.Config.IsAnswerFinal)
            {
                var existingAnswer = await _repository.GetPollAnswer(participant, pollId);
                if (existingAnswer != null)
                    throw PollError.AnswerAlreadySubmitted.ToException();
            }

            var key = Guid.NewGuid().ToString("N");

            await _repository.SetPollAnswer(participant, pollId, new PollAnswerWithKey(answer, key));

            poll = await _repository.GetPoll(participant.ConferenceId, pollId);
            if (poll == null)
            {
                // optimistic concurrency
                await _repository.DeletePollAnswers(participant.ConferenceId, pollId);
                throw PollError.PollNotFound.ToException();
            }

            await _mediator.Send(new UpdateSynchronizedObjectRequest(participant.ConferenceId,
                SynchronizedPollAnswers.SyncObjId(participant.Id)));
            await _mediator.Send(new UpdateSynchronizedObjectRequest(participant.ConferenceId,
                SynchronizedPollResult.SyncObjId(pollId)));

            return Unit.Value;
        }
    }
}
