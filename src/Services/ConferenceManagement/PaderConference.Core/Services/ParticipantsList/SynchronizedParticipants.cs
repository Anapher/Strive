using System.Collections.Immutable;

namespace PaderConference.Core.Services.ParticipantsList
{
    public record SynchronizedParticipants(IImmutableDictionary<string, ParticipantData> Participants);
}
