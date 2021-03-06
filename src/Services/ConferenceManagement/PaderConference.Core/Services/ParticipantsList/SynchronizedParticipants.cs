using System.Collections.Generic;

namespace PaderConference.Core.Services.ParticipantsList
{
    public record SynchronizedParticipants(IReadOnlyDictionary<string, ParticipantData> Participants);
}
