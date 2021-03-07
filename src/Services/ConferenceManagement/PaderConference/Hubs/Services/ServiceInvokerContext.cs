using Autofac;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Services;

namespace PaderConference.Hubs.Services
{
    public record ServiceInvokerContext(Hub Hub, ILifetimeScope Context, Participant Participant);
}
