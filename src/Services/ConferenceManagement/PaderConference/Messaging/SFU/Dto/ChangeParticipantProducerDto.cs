using PaderConference.Core.Services.Media.Dtos;

namespace PaderConference.Messaging.SFU.Dto
{
    public record ChangeParticipantProducerDto(string ParticipantId, ProducerSource Source, MediaStreamAction Action);
}
