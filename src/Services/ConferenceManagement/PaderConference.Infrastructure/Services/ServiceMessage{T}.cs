using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services;

namespace PaderConference.Infrastructure.Services
{
    public class ServiceMessage<TPayload> : ServiceMessage, IServiceMessage<TPayload>
    {
        public ServiceMessage(TPayload payload, Participant participant, HubCallerContext context,
            IHubCallerClients clients) : base(participant, context, clients)
        {
            Payload = payload;
        }

        public TPayload Payload { get; }
    }
}