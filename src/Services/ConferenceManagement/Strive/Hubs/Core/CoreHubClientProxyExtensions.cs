using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR;
using Strive.Hubs.Core.Responses;

namespace Strive.Hubs.Core
{
    public static class CoreHubClientProxyExtensions
    {
        public static Task OnRequestDisconnect(this IClientProxy clientProxy, RequestDisconnectDto payload,
            CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.OnRequestDisconnect, payload, token);
        }

        public static Task OnSynchronizeObjectState(this IClientProxy clientProxy, SyncObjPayload<object> payload,
            CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.OnSynchronizeObjectState, payload, token);
        }

        public static Task OnSynchronizedObjectUpdated(this IClientProxy clientProxy,
            SyncObjPayload<JsonPatchDocument> payload, CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.OnSynchronizedObjectUpdated, payload, token);
        }

        public static Task ChatMessage(this IClientProxy clientProxy, ChatMessageDto message,
            CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.ChatMessage, message, token);
        }

        public static Task EquipmentError(this IClientProxy clientProxy, EquipmentErrorDto message,
            CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.OnEquipmentError, message, token);
        }
    }
}
