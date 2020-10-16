namespace PaderConference.Infrastructure
{
    public static class CoreHubMessages
    {
        public static class Request
        {
            public const string SendChatMessage = nameof(SendChatMessage);
            public const string RequestChat = nameof(RequestChat);
            public const string RequestParticipants = nameof(RequestParticipants);

            public const string RequestRouterCapabilities = nameof(RequestRouterCapabilities);
        }

        public static class Response
        {
            public const string OnSynchronizedObjectUpdated = nameof(OnSynchronizedObjectUpdated);
            public const string OnSynchronizeObjectState = nameof(OnSynchronizeObjectState);

            public const string ChatMessage = nameof(ChatMessage);
            public const string Chat = nameof(Chat);
            public const string OnUserJoined = nameof(OnUserJoined);
            public const string OnConferenceDoesNotExist = nameof(OnConferenceDoesNotExist);
            public const string OnConferenceNotStarted = nameof(OnConferenceNotStarted);

            public const string OnParticipantsUpdated = nameof(OnParticipantsUpdated);

            public const string OnIceCandidate = nameof(OnIceCandidate);
            public const string OnSdp = nameof(OnSdp);

            public const string OnRouterCapabilities = nameof(OnRouterCapabilities);
        }
    }
}