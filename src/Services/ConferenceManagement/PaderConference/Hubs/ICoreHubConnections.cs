using System.Diagnostics.CodeAnalysis;

namespace PaderConference.Hubs
{
    public interface ICoreHubConnections
    {
        void SetParticipant(string participantId, ParticipantConnection connection);

        void RemoveParticipant(string participantId);

        bool TryGetParticipant(string participantId, [NotNullWhen(true)] out ParticipantConnection? connection);
    }
}
