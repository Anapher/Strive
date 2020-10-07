using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace PaderConference.Infrastructure.Services.Chat.Filters
{
    /// <summary>
    ///     Send the message to all participants except the ones given
    /// </summary>
    public class ExcludeFilter : IMessageFilter
    {
        private readonly string[] _exclude;

        public ExcludeFilter(string[] exclude)
        {
            _exclude = exclude;
        }

        public bool ShowMessageTo(string connectionId)
        {
            return !_exclude.Contains(connectionId);
        }

        public IClientProxy SendTo(IHubCallerClients clients, string groupName)
        {
            return clients.GroupExcept(groupName, _exclude);
        }
    }
}