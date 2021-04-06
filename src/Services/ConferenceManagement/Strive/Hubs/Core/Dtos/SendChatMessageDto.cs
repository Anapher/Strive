using Strive.Core.Services.Chat.Requests;

namespace Strive.Hubs.Core.Dtos
{
    public record SendChatMessageDto(string Message, string Channel, ChatMessageOptions Options);
}
