using Strive.Core.Services.Media.Dtos;

namespace Strive.Messaging.SFU.Dto
{
    public record ChangeParticipantProducerDto(string ParticipantId, ProducerSource Source, MediaStreamAction Action);
}
