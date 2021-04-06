using System;
using Strive.Core.Services.Chat;
using Strive.Core.Services.Chat.Requests;

namespace Strive.Hubs.Core.Responses
{
    public record ChatMessageDto(int Id, string Channel, ChatMessageSender? Sender, string Message,
        DateTimeOffset Timestamp, ChatMessageOptions Options);
}
