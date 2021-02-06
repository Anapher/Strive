using PaderConference.Core.Services.Media.Mediasoup;

namespace PaderConference.Core.Services.Media.Communication
{
    public record ChangeProducerSourceRequest(ProducerSource Source, StreamAction Action);
}
