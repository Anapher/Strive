using Autofac;
using Microsoft.AspNetCore.SignalR;

namespace PaderConference.Hubs.Services
{
    public record ServiceInvokerContext(Hub Hub, ILifetimeScope Context, string ConferenceId, string ParticipantId);
}
