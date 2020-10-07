using PaderConference.Infrastructure.Services.Chat;

namespace PaderConference.Infrastructure.Hubs.Dto
{
    public class SendChatMessageDto
    {
        public string? Message { get; set; }

        public ChatMessageFilter? Filter { get; set; }
    }
}