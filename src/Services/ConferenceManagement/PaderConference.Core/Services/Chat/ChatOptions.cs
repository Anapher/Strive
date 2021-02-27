namespace PaderConference.Core.Services.Chat
{
    public record ChatOptions
    {
        /// <summary>
        ///     If the participant is currently noted typing and the info that he is typing exceeds this value in seconds, it will
        ///     be automatically updated to not typing
        /// </summary>
        public double CancelParticipantIsTypingAfter { get; set; } = 30;

        /// <summary>
        ///     If true, synchronize the state of participants that are currently typing
        /// </summary>
        public bool ShowTyping { get; set; } = true;

        public bool IsRoomChatEnabled { get; set; } = true;
        public bool IsGlobalChatEnabled { get; set; } = true;
        public bool IsPrivateChatEnabled { get; set; } = true;
        public bool CanSendAnonymousMessage { get; set; }
    }
}
