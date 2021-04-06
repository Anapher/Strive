using Autofac;
using Microsoft.AspNetCore.SignalR;
using Strive.Core.Services;

namespace Strive.Hubs.Core.Services
{
    public record ServiceInvokerContext(Hub Hub, ILifetimeScope Context, Participant Participant);
}
