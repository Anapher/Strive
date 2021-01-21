using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Signaling;

namespace PaderConference.Core.Services.Chat.Filters
{
    /// <summary>
    ///     Send the message only to determined participants
    /// </summary>
    public class IncludeFilter : IMessageFilter
    {
        private readonly string[] _include;

        public IncludeFilter(IEnumerable<string> include, string senderParticipantId)
        {
            _include = include.Concat(new[] {senderParticipantId}).ToArray(); // also show this message to the sender
        }

        public bool ShowMessageTo(string participantId)
        {
            return _include.Contains(participantId);
        }

        public async Task SendAsync(ISignalMessenger clients, IConnectionMapping connectionMapping, string conferenceId,
            string method, object dto)
        {
            foreach (var participantId in _include)
                if (connectionMapping.ConnectionsR.TryGetValue(participantId, out var connection))
                    await clients.SendToConnectionAsync(connection.MainConnectionId, method, dto);
        }
    }
}