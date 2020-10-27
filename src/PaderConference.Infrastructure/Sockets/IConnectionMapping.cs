using System.Collections.Concurrent;
using System.Collections.Generic;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Sockets
{
    public interface IConnectionMapping
    {
        ConcurrentDictionary<string, Participant> Connections { get; }

        ConcurrentDictionary<Participant, IParticipantConnections> ConnectionsR { get; }

        bool Add(string connectionId, Participant participant, bool equipment = false);

        bool Remove(string connectionId);
    }

    public interface IParticipantConnections
    {
        string MainConnectionId { get; }

        IEnumerable<string> Equipment { get; }
    }

    public class ParticipantConnections : IParticipantConnections
    {
        public ParticipantConnections(string mainConnectionId)
        {
            MainConnectionId = mainConnectionId;
        }

        public string MainConnectionId { get; }

        public readonly ConcurrentDictionary<string, byte> Equipment = new ConcurrentDictionary<string, byte>();

        IEnumerable<string> IParticipantConnections.Equipment => Equipment.Keys;
    }
}