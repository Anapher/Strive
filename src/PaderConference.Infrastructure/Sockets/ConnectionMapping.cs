using System.Collections.Concurrent;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Sockets
{
    public class ConnectionMapping : IConnectionMapping
    {
        public ConcurrentDictionary<string, Participant> Connections { get; } =
            new ConcurrentDictionary<string, Participant>();

        public ConcurrentDictionary<Participant, string> ConnectionsR { get; } =
            new ConcurrentDictionary<Participant, string>();

        public bool Add(string connectionId, Participant participant)
        {
            if (!Connections.TryAdd(connectionId, participant))
                return false;

            if (!ConnectionsR.TryAdd(participant, connectionId))
            {
                Connections.TryRemove(connectionId, out _);
                return false;
            }

            return true;
        }

        public bool Remove(string connectionId)
        {
            if (!Connections.TryRemove(connectionId, out var participant))
                return false;

            ConnectionsR.TryRemove(participant, out _);

            return true;
        }

        public bool Remove(Participant participant)
        {
            if (!ConnectionsR.TryRemove(participant, out var connectionId))
                return false;

            Connections.TryRemove(connectionId, out _);

            return true;
        }
    }
}