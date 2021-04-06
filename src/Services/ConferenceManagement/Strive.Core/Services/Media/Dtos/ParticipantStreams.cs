using System.Collections.Generic;

namespace Strive.Core.Services.Media.Dtos
{
    public record ParticipantStreams(IReadOnlyDictionary<string, ConsumerInfo> Consumers,
        IReadOnlyDictionary<ProducerSource, ProducerInfo> Producers);
}
