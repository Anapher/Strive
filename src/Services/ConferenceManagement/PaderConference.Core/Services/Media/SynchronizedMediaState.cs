using System.Collections.Generic;
using PaderConference.Core.Services.Media.Dtos;

namespace PaderConference.Core.Services.Media
{
    public record SynchronizedMediaState(IReadOnlyDictionary<string, ParticipantStreams> Streams);
}
