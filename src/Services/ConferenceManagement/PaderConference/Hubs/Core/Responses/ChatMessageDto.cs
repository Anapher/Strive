using System;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Requests;

namespace PaderConference.Hubs.Core.Responses
{
    public record ChatMessageDto(int Id, string Channel, ChatMessageSender? Sender, string Message,
        DateTimeOffset Timestamp, ChatMessageOptions Options);
}
