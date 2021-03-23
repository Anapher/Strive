using MediatR;
using PaderConference.Core.Services.Media.Dtos;

namespace PaderConference.Core.Services.Media.Requests
{
    public record ChangeParticipantProducerRequest(Participant Participant, ProducerSource Source,
        MediaStreamAction Action) : IRequest;
}
