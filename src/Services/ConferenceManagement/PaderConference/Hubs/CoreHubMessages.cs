using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Hubs.Responses;
using PermissionsDict = System.Collections.Generic.Dictionary<string, Newtonsoft.Json.Linq.JValue>;

namespace PaderConference.Hubs
{
    public static class CoreHubMessages
    {
        public static class Request
        {
            public const string SendChatMessage = "SendChatMessage";
            public const string RequestChat = "RequestChat";
        }

        public static class Response
        {
            public const string OnError = "OnError";

            public const string OnConnectionError = "OnConnectionError";

            public const string OnSynchronizedObjectUpdated = "OnSynchronizedObjectUpdated";
            public const string OnSynchronizeObjectState = "OnSynchronizeObjectState";

            public const string ChatMessage = "ChatMessage";

            public const string OnPermissionsUpdated = "OnPermissionsUpdated";
            public const string OnEquipmentUpdated = "OnEquipmentUpdated";
            public const string OnEquipmentCommand = "OnEquipmentCommand";

            public const string OnRequestDisconnect = "OnRequestDisconnect";
        }
    }

    public static class CoreHubMessage
    {
        public static Task OnPermissionsUpdated(this IClientProxy clientProxy, PermissionsDict payload,
            CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.Response.OnPermissionsUpdated, payload, token);
        }

        public static Task OnRequestDisconnect(this IClientProxy clientProxy, RequestDisconnectDto payload,
            CancellationToken token = default)
        {
            return clientProxy.SendAsync(CoreHubMessages.Response.OnRequestDisconnect, payload, token);
        }
    }
}
