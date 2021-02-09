using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace PaderConference.Hubs
{
    public class CoreHubConnections : ICoreHubConnections
    {
        private readonly ConcurrentDictionary<string, ParticipantConnection> _connections = new();

        public void SetParticipant(string participantId, ParticipantConnection connection)
        {
            _connections[participantId] = connection;
        }

        public void RemoveParticipant(string participantId)
        {
            _connections.TryRemove(participantId, out _);
        }

        public bool TryGetParticipant(string participantId, [NotNullWhen(true)] out ParticipantConnection? connection)
        {
            return _connections.TryGetValue(participantId, out connection);
        }
    }

    public interface ICoreHubConnections
    {
        void SetParticipant(string participantId, ParticipantConnection connection);

        void RemoveParticipant(string participantId);

        bool TryGetParticipant(string participantId, [NotNullWhen(true)] out ParticipantConnection? connection);
    }

    public record ParticipantConnection(string ConferenceId, string ConnectionId);
}
