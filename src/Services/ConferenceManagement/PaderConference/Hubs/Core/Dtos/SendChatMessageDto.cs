using PaderConference.Core.Services.Chat.Requests;

namespace PaderConference.Hubs.Core.Dtos
{
    public record SendChatMessageDto(string Message, string Channel, ChatMessageOptions Options);
}
