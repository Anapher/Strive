using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Signaling;

namespace PaderConference.Core.Services.Chat.Filters
{
    /// <summary>
    ///     Send the message only to determined participants
    /// </summary>
    public class IncludeFilter : IMessageFilter
    {
        private readonly string[] _include;

        public IncludeFilter(IEnumerable<string> include, string senderConnectionId)
        {
            _include = include.Concat(new[] {senderConnectionId}).ToArray(); // also show this message to the sender
        }

        public bool ShowMessageTo(string connectionId)
        {
            return _include.Contains(connectionId);
        }

        public async Task SendAsync(ISignalMessenger clients, string conferenceId, string method, object dto)
        {
            foreach (var connectionId in _include) await clients.SendToConnectionAsync(connectionId, method, dto);
        }
    }
}