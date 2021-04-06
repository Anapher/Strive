using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.ConferenceControl.Requests;

namespace Strive.Core.Services.ConferenceControl.UseCases
{
    public class CheckIsParticipantJoinedUseCase : IRequestHandler<CheckIsParticipantJoinedRequest, bool>
    {
        private readonly IJoinedParticipantsRepository _repository;

        public CheckIsParticipantJoinedUseCase(IJoinedParticipantsRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(CheckIsParticipantJoinedRequest request, CancellationToken cancellationToken)
        {
            return await _repository.IsParticipantJoined(request.Participant);
        }
    }
}
