using System.Collections.Concurrent;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Sockets
{
    public interface IConnectionMapping
    {
        ConcurrentDictionary<string, Participant> Connections { get; }
        ConcurrentDictionary<Participant, string> ConnectionsR { get; }
        bool Add(string connectionId, Participant participant);
        bool Remove(string connectionId);
        bool Remove(Participant participant);
    }
}