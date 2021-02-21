using System.Collections.Immutable;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services.ConferenceControl
{
    public record SynchronizedParticipants(IImmutableList<ParticipantData> Participants);
}
