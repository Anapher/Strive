using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services;

namespace PaderConference.Infrastructure.ServiceFactories.Base
{
    public class ServiceMessage : IServiceMessage
    {
        private readonly HubCallerContext _context;
        private readonly IHubCallerClients _clients;

        public ServiceMessage(Participant participant, HubCallerContext context, IHubCallerClients clients)
        {
            _context = context;
            _clients = clients;
            Participant = participant;
        }

        public Participant Participant { get; }
        public string ConnectionId => _context.ConnectionId;

        public Task SendToCallerAsync(string method, object dto)
        {
            return _clients.Caller.SendAsync(method, dto);
        }
    }
}