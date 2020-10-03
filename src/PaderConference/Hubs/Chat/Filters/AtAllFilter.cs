using Microsoft.AspNetCore.SignalR;

namespace PaderConference.Hubs.Chat.Filters
{
    /// <summary>
    ///     Send the message to all participants
    /// </summary>
    public class AtAllFilter : IMessageFilter
    {
        public bool ShowMessageTo(string connectionId)
        {
            return true;
        }

        public IClientProxy SendTo(IHubCallerClients clients, string groupName)
        {
            return clients.Group(groupName);
        }
    }
}