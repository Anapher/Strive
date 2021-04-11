using System.Diagnostics.CodeAnalysis;

namespace Strive.Hubs.Core
{
    public interface ICoreHubConnections
    {
        void SetParticipant(string participantId, ParticipantConnection connection);

        void RemoveParticipant(string participantId);

        bool TryRemoveParticipant(string participantId, ParticipantConnection connection);

        bool TryGetParticipant(string participantId, [NotNullWhen(true)] out ParticipantConnection? connection);
    }
}
