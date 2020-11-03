using System.Collections.Concurrent;
using System.Collections.Generic;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IConnectionMapping
    {
        ConcurrentDictionary<string, Participant> Connections { get; }

        ConcurrentDictionary<string, IParticipantConnections> ConnectionsR { get; }

        bool Add(string connectionId, Participant participant, bool equipment = false);

        bool Remove(string connectionId);
    }

    public interface IParticipantConnections
    {
        string MainConnectionId { get; }

        IEnumerable<string> Equipment { get; }
    }
}
