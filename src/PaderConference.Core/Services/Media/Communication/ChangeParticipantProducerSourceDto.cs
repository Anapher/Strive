using PaderConference.Core.Services.Media.Mediasoup;

namespace PaderConference.Core.Services.Media.Communication
{
    public record ChangeParticipantProducerSourceDto(string ParticipantId, ProducerSource Source, StreamAction Action);
}
