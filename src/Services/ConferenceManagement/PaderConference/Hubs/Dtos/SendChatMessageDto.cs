using PaderConference.Core.Services.Chat.Requests;

namespace PaderConference.Hubs.Dtos
{
    public record SendChatMessageDto(string Message, string Channel, ChatMessageOptions Options);
}
