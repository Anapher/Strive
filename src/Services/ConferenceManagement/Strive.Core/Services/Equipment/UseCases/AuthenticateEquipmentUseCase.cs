using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.ConferenceControl.Requests;
using Strive.Core.Services.Equipment.Gateways;
using Strive.Core.Services.Equipment.Requests;

namespace Strive.Core.Services.Equipment.UseCases
{
    public class AuthenticateEquipmentUseCase : IRequestHandler<AuthenticateEquipmentRequest>
    {
        private readonly IEquipmentTokenRepository _tokenRepository;
        private readonly IMediator _mediator;

        public AuthenticateEquipmentUseCase(IEquipmentTokenRepository tokenRepository, IMediator mediator)
        {
            _tokenRepository = tokenRepository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(AuthenticateEquipmentRequest request, CancellationToken cancellationToken)
        {
            var actualToken = await _tokenRepository.Get(request.Participant);
            if (actualToken != request.Token)
                throw EquipmentError.InvalidToken.ToException();

            if (!await _mediator.Send(new CheckIsParticipantJoinedRequest(request.Participant)))
                throw EquipmentError.ParticipantNotJoined.ToException();

            return Unit.Value;
        }
    }
}
