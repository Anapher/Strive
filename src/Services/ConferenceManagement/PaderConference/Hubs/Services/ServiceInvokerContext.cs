using Autofac;
using Microsoft.AspNetCore.SignalR;

namespace PaderConference.Hubs.Services
{
    public record ServiceInvokerContext(Hub Hub, IComponentContext Context, string ConferenceId, string ParticipantId);
}
