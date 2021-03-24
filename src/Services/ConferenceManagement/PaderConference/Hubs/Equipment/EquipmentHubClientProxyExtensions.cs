using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Hubs.Core;
using PaderConference.Hubs.Equipment.Responses;

namespace PaderConference.Hubs.Equipment
{
    public static class EquipmentHubClientProxyExtensions
    {
        public static Task SendEquipmentCommand(this IClientProxy clientProxy, EquipmentCommandDto dto,
            CancellationToken token = default)
        {
            return clientProxy.SendAsync(EquipmentHubMessages.OnEquipmentCommand, dto, token);
        }

        public static Task OnRequestDisconnect(this IClientProxy clientProxy, CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.OnRequestDisconnect, token);
        }
    }
}
