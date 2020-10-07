using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace PaderConference.Infrastructure.Services.Chat.Filters
{
    /// <summary>
    ///     Send the message only to determined participants
    /// </summary>
    public class IncludeFilter : IMessageFilter
    {
        private readonly string[] _include;

        public IncludeFilter(string[] include, string senderConnectionId)
        {
            _include = include.Concat(new[] {senderConnectionId}).ToArray(); // also show this message to the sender
        }

        public bool ShowMessageTo(string connectionId)
        {
            return _include.Contains(connectionId);
        }

        public IClientProxy SendTo(IHubCallerClients clients, string groupName)
        {
            return clients.Clients(_include);
        }
    }
}