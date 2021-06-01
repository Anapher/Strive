using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Poll.Utilities;

namespace Strive.Core.Services.Poll.UseCase
{
    public class FetchPollResultsUseCase : IRequestHandler<FetchPollResultsRequest, SanitizedPollResult>
    {
        private readonly IPollRepository _repository;
        private readonly IComponentContext _context;

        public FetchPollResultsUseCase(IPollRepository repository, IComponentContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<SanitizedPollResult> Handle(FetchPollResultsRequest request,
            CancellationToken cancellationToken)
        {
            var (conferenceId, pollId) = request;

            var poll = await _repository.GetPoll(conferenceId, pollId);
            if (poll == null)
                throw PollError.PollNotFound.ToException();

            var answers = await _repository.GetPollAnswers(conferenceId, poll.Id);

            var wrapper = new PollAnswerAggregatorWrapper(_context);
            var result = await wrapper.AggregateAnswers(poll.Instruction, answers.Values);

            IReadOnlyDictionary<string, string>? participantIdTranslationTable = null;
            if (!poll.Config.IsAnonymous)
            {
                participantIdTranslationTable = answers.ToDictionary(x => x.Value.Key, x => x.Key);
            }

            return new SanitizedPollResult(result, participantIdTranslationTable);
        }
    }
}
