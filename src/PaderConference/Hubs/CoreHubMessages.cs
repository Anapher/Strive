namespace PaderConference.Hubs
{
    public static class CoreHubMessages
    {
        public static class Request
        {
            public const string SendChatMessage = nameof(SendChatMessage);
            public const string RequestChat = nameof(RequestChat);
        }

        public static class Response
        {
            public const string ChatMessage = nameof(ChatMessage);
            public const string Chat = nameof(Chat);
            public const string OnUserJoined = nameof(OnUserJoined);
        }
    }
}