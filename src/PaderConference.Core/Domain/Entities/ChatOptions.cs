namespace PaderConference.Core.Domain.Entities
{
    public class ChatOptions
    {
        /// <summary>
        ///     Allow the participants to send private message (to include or exclude specific participants)
        /// </summary>
        public bool AllowPrivateConversation { get; set; } = true;

        /// <summary>
        ///     The amount of messages that should be remembered
        /// </summary>
        public int MessageHistoryCount { get; set; } = 1000;
    }
}