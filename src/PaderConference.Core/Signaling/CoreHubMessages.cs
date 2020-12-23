namespace PaderConference.Core.Signaling
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
}
