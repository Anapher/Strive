using System.Collections.Concurrent;
using System.Collections.Generic;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Infrastructure.Sockets
{
    public class ParticipantConnections : IParticipantConnections
    {
        public ParticipantConnections(string mainConnectionId)
        {
            MainConnectionId = mainConnectionId;
        }

        public string MainConnectionId { get; }

        public readonly ConcurrentDictionary<string, byte> Equipment = new();

        IEnumerable<string> IParticipantConnections.Equipment => Equipment.Keys;
    }
}