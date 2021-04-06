using Strive.Core.Services;

namespace Strive.Contracts
{
    public interface ParticipantKicked
    {
        Participant Participant { get; }

        string? ConnectionId { get; }
    }
}
