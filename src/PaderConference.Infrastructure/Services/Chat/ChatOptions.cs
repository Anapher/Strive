namespace PaderConference.Infrastructure.Services.Chat
{
    public class ChatOptions
    {
        /// <summary>
        ///     The maximum amount of messages that will be saved. If this amount is exceeded, for every new message the oldest
        ///     message is removed.
        /// </summary>
        public int MaxChatMessageHistory { get; set; } = 500;

        /// <summary>
        ///     If the participant is currently noted typing and the info that he is typing exceeds this value in seconds, it will
        ///     be automatically updated to not typing
        /// </summary>
        public int CancelParticipantIsTypingAfter { get; set; } = 30;

        /// <summary>
        ///     The interval in seconds of the timer that will check if any participant is typing and exceeds
        ///     <see cref="CancelParticipantIsTypingAfter" />
        /// </summary>
        public int CancelParticipantIsTypingInterval { get; set; } = 5;
    }
}