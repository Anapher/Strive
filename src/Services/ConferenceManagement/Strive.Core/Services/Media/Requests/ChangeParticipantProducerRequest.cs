using MediatR;
using Strive.Core.Services.Media.Dtos;

namespace Strive.Core.Services.Media.Requests
{
    public record ChangeParticipantProducerRequest(Participant Participant, ProducerSource Source,
        MediaStreamAction Action) : IRequest;
}
