using System.Collections.Concurrent;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Infrastructure.Sockets
{
    public class ConnectionMapping : IConnectionMapping
    {
        public ConcurrentDictionary<string, ParticipantData> Connections { get; } = new();

        public ConcurrentDictionary<string, IParticipantConnections> ConnectionsR { get; } =
            new ConcurrentDictionary<string, IParticipantConnections>();

        public bool Add(string connectionId, ParticipantData participantData, bool equipment)
        {
            if (!Connections.TryAdd(connectionId, participantData))
                return false;

            if (equipment)
            {
                // receive participant connections object
                if (!ConnectionsR.TryGetValue(participantData.ParticipantId, out var connections))
                {
                    Connections.TryRemove(connectionId, out _);
                    return false;
                }

                // add connection id to equipment
                if (!((ParticipantConnections) connections).Equipment.TryAdd(connectionId, default))
                {
                    Connections.TryRemove(connectionId, out _);
                    return false;
                }
            }
            else
            {
                // create participant connections object and try to add
                var connections = new ParticipantConnections(connectionId);
                if (!ConnectionsR.TryAdd(participantData.ParticipantId, connections))
                {
                    Connections.TryRemove(connectionId, out _);
                    return false;
                }
            }

            return true;
        }

        public bool Remove(string connectionId)
        {
            if (!Connections.TryRemove(connectionId, out var participant))
                return false;

            if (ConnectionsR.TryGetValue(participant.ParticipantId, out var connections))
            {
                if (connections.MainConnectionId == connectionId)
                    ConnectionsR.TryRemove(participant.ParticipantId, out _);
                else
                    ((ParticipantConnections) connections).Equipment.TryRemove(connectionId, out _);
            }

            return true;
        }
    }
}