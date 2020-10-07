using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services
{
    public interface IServiceMessage
    {
        Participant Participant { get; }

        HubCallerContext Context { get; }

        IHubCallerClients Clients { get; }
    }
}