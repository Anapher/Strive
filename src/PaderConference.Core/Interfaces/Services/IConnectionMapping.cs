using System.Collections.Concurrent;
using System.Collections.Generic;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IConnectionMapping
    {
        /// <summary>
        ///     Map connection ids to participant
        /// </summary>
        ConcurrentDictionary<string, Participant> Connections { get; }

        /// <summary>
        ///     Map participant ids to connections
        /// </summary>
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
