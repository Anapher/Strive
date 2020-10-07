using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services
{
    public class ServiceMessage : IServiceMessage
    {
        public ServiceMessage(Participant participant, HubCallerContext context, IHubCallerClients clients)
        {
            Participant = participant;
            Context = context;
            Clients = clients;
        }

        public Participant Participant { get; }
        public HubCallerContext Context { get; }
        public IHubCallerClients Clients { get; }
    }
}