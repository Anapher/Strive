using PaderConference.Core.Services.Chat.Dto;

namespace PaderConference.Core.Services.Chat.Requests
{
    public record SendChatMessageRequest(string Message, SendingMode? Mode);
}