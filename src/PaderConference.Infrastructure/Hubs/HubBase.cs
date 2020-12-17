using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.Hubs
{
    public abstract class HubBase : Hub
    {
        protected readonly ILogger Logger;
        protected readonly IConnectionMapping ConnectionMapping;

        protected HubBase(IConnectionMapping connectionMapping, ILogger logger)
        {
            ConnectionMapping = connectionMapping;
            Logger = logger;
        }

        protected IServiceMessage CreateMessage(Participant participant)
        {
            return new ServiceMessage(participant, Context, Clients);
        }

        protected IServiceMessage<T> CreateMessage<T>(T payload, Participant participant)
        {
            return new ServiceMessage<T>(payload, participant, Context, Clients);
        }

        protected bool AssertParticipant([NotNullWhen(true)] out Participant? participant)
        {
            if (!ConnectionMapping.Connections.TryGetValue(Context.ConnectionId, out participant))
            {
                Logger.LogWarning("Connection {connectionId} is not mapped to a participant.", Context.ConnectionId);
                return false;
            }

            return true;
        }
    }
}