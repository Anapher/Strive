using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Services;
using PaderConference.Infrastructure.Sockets;

namespace PaderConference.Infrastructure.Hubs
{
    public abstract class HubBase : Hub
    {
        private readonly ILogger _logger;
        protected readonly IConnectionMapping ConnectionMapping;

        protected HubBase(IConnectionMapping connectionMapping, ILogger logger)
        {
            ConnectionMapping = connectionMapping;
            _logger = logger;
        }

        protected bool GetMessage([NotNullWhen(true)] out IServiceMessage? serviceMessage)
        {
            if (AssertParticipant(out var participant))
            {
                serviceMessage = new ServiceMessage(participant, Context, Clients);
                return true;
            }

            serviceMessage = null;
            return false;
        }

        protected bool GetMessage<T>(T payload, [NotNullWhen(true)] out IServiceMessage<T>? serviceMessage)
        {
            if (AssertParticipant(out var participant))
            {
                serviceMessage = new ServiceMessage<T>(payload, participant, Context, Clients);
                return true;
            }

            serviceMessage = null;
            return false;
        }

        protected bool AssertParticipant([NotNullWhen(true)] out Participant? participant)
        {
            if (!ConnectionMapping.Connections.TryGetValue(Context.ConnectionId, out participant))
            {
                _logger.LogWarning("Connection {connectionId} is not mapped to a participant.", Context.ConnectionId);
                return false;
            }

            return true;
        }
    }
}