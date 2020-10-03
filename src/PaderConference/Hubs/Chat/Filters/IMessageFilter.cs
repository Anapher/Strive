using Microsoft.AspNetCore.SignalR;

namespace PaderConference.Hubs.Chat.Filters
{
    public interface IMessageFilter
    {
        bool ShowMessageTo(string connectionId);
        IClientProxy SendTo(IHubCallerClients clients, string groupName);
    }
}