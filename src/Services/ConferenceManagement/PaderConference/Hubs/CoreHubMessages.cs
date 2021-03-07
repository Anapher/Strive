namespace PaderConference.Hubs
{
    public static class CoreHubMessages
    {
        public static class Response
        {
            public const string OnConnectionError = "OnConnectionError";

            public const string OnSynchronizedObjectUpdated = "OnSynchronizedObjectUpdated";
            public const string OnSynchronizeObjectState = "OnSynchronizeObjectState";

            public const string ChatMessage = "ChatMessage";

            public const string OnEquipmentUpdated = "OnEquipmentUpdated";
            public const string OnEquipmentCommand = "OnEquipmentCommand";

            public const string OnRequestDisconnect = "OnRequestDisconnect";
        }
    }
}
