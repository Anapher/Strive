using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Media.Requests;
using PaderConference.Messaging.SFU.Dto;

namespace PaderConference.Messaging.SFU.UseCase
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
