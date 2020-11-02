using System.Collections.Concurrent;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Infrastructure.Sockets
{
    public class ConnectionMapping : IConnectionMapping
    {
        public ConcurrentDictionary<string, Participant> Connections { get; } =
            new ConcurrentDictionary<string, Participant>();

        public ConcurrentDictionary<Participant, IParticipantConnections> ConnectionsR { get; } =
            new ConcurrentDictionary<Participant, IParticipantConnections>();

        public bool Add(string connectionId, Participant participant, bool equipment)
        {
            if (!Connections.TryAdd(connectionId, participant))
                return false;

            if (equipment)
            {
                // receive participant connections object
                if (!ConnectionsR.TryGetValue(participant, out var connections))
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
                if (!ConnectionsR.TryAdd(participant, connections))
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

            if (ConnectionsR.TryGetValue(participant, out var connections))
            {
                if (connections.MainConnectionId == connectionId)
                    ConnectionsR.TryRemove(participant, out _);
                else
                    ((ParticipantConnections) connections).Equipment.TryRemove(connectionId, out _);
            }

            return true;
        }
    }
}