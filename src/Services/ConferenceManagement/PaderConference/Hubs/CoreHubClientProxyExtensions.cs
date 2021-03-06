using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using PaderConference.Hubs.Responses;

namespace PaderConference.Hubs
{
    public static class CoreHubClientProxyExtensions
    {
        public static Task OnPermissionsUpdated(this IClientProxy clientProxy, Dictionary<string, JValue> payload,
            CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.Response.OnPermissionsUpdated, payload, token);
        }

        public static Task OnRequestDisconnect(this IClientProxy clientProxy, RequestDisconnectDto payload,
            CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.Response.OnRequestDisconnect, payload, token);
        }

        public static Task OnSynchronizeObjectState(this IClientProxy clientProxy, SyncObjPayload<object> payload,
            CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.Response.OnSynchronizeObjectState, payload, token);
        }

        public static Task OnSynchronizedObjectUpdated(this IClientProxy clientProxy,
            SyncObjPayload<JsonPatchDocument> payload, CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.Response.OnSynchronizedObjectUpdated, payload, token);
        }
    }
}
