using System.Collections.Generic;

namespace PaderConference.Core.Services.Media.Dtos
{
    public record ParticipantStreams(IReadOnlyDictionary<string, ConsumerInfo> Consumers,
        IReadOnlyDictionary<ProducerSource, ProducerInfo> Producers);
}
