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
        ConcurrentDictionary<string, ParticipantData> Connections { get; }

        /// <summary>
        ///     Map participant ids to connections
        /// </summary>
        ConcurrentDictionary<string, IParticipantConnections> ConnectionsR { get; }

        bool Add(string connectionId, ParticipantData participantData, bool equipment = false);

        bool Remove(string connectionId);
    }

    public interface IParticipantConnections
    {
        /// <summary>
        ///     The connection id of the primary connection of this participant (the GUI)
        /// </summary>
        string MainConnectionId { get; }

        IEnumerable<string> Equipment { get; }
    }
}
