using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Media.Requests;
using Strive.Messaging.SFU.Dto;

namespace Strive.Messaging.SFU.UseCase
{
    public class ChangeParticipantProducerUseCase : IRequestHandler<ChangeParticipantProducerRequest>
    {
        private readonly ISfuNotifier _sfuNotifier;

        public ChangeParticipantProducerUseCase(ISfuNotifier sfuNotifier)
        {
            _sfuNotifier = sfuNotifier;
        }


        public async Task<Unit> Handle(ChangeParticipantProducerRequest request, CancellationToken cancellationToken)
        {
            await _sfuNotifier.ChangeProducer(request.Participant.ConferenceId,
                new ChangeParticipantProducerDto(request.Participant.Id, request.Source, request.Action));

            return Unit.Value;
        }
    }
}
