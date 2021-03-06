using PaderConference.Core.Services;

namespace PaderConference.Contracts
{
    public interface ParticipantKicked
    {
        Participant Participant { get; }

        string? ConnectionId { get; }
    }
}
