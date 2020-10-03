using PaderConference.Hubs.Chat;

namespace PaderConference.Models.Signal
{
    public class SendChatMessageDto
    {
        public string? Message { get; set; }

        public ChatMessageFilter? Filter { get; set; }
    }
}