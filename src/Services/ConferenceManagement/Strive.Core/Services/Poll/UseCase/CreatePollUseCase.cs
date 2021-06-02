using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Poll.Utilities;

namespace Strive.Core.Services.Poll.UseCase
{
    public class CreatePollUseCase : IRequestHandler<CreatePollRequest, string>
    {
        private readonly IMediator _mediator;
        private readonly IPollRepository _repository;
        private readonly IOpenConferenceRepository _openConferenceRepository;

        public CreatePollUseCase(IPollRepository repository, IOpenConferenceRepository openConferenceRepository,
            IMediator mediator)
        {
            _repository = repository;
            _openConferenceRepository = openConferenceRepository;
            _mediator = mediator;
        }

        public async Task<string> Handle(CreatePollRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, pollInstruction, pollConfig, initialState, roomId) = request;

            var pollId = Guid.NewGuid().ToString("N");
            var poll = new Poll(pollId, pollInstruction, pollConfig, roomId);

            await _repository.CreatePoll(conferenceId, poll);
            await _repository.SetPollState(conferenceId, pollId, initialState);

            if (!await _openConferenceRepository.IsOpen(request.ConferenceId))
            {
                await _repository.DeletePollAndState(conferenceId, pollId);
                throw ConferenceError.ConferenceNotOpen.ToException();
            }

            await _mediator.Send(new UpdateParticipantSubscriptionsOfPollRequest(conferenceId, poll));
            await SceneUpdater.UpdateScene(_mediator, conferenceId, roomId);

            return pollId;
        }
    }
}
