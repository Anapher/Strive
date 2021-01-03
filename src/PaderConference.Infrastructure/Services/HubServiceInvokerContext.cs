using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services;

namespace PaderConference.Infrastructure.Services
{
    public class HubServiceInvokerContext : IServiceInvokerContext
    {
        private readonly Hub _hub;

        public HubServiceInvokerContext(Hub hub)
        {
            _hub = hub;
        }

        public string ConnectionId => _hub.Context.ConnectionId;

        public IServiceMessage CreateMessage(Participant participant)
        {
            return new ServiceMessage(participant, _hub.Context, _hub.Clients);
        }

        public IServiceMessage<T> CreateMessage<T>(T payload, Participant participant)
        {
            return new ServiceMessage<T>(payload, participant, _hub.Context, _hub.Clients);
        }
    }
}
