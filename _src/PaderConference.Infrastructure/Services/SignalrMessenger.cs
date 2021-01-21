using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Signaling;

namespace PaderConference.Infrastructure.Services
{
    public class SignalrMessenger<THub> : ISignalMessenger where THub : Hub
    {
        private readonly IHubContext<THub> _context;

        public SignalrMessenger(IHubContext<THub> context)
        {
            _context = context;
        }

        public Task SendToConferenceAsync(string conferenceId, string method, object? arg)
        {
            return _context.Clients.Group(conferenceId).SendAsync(method, arg);
        }

        public Task SendToConnectionAsync(string connectionId, string method, object? arg)
        {
            return _context.Clients.Client(connectionId).SendAsync(method, arg);
        }
    }
}
