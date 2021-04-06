using System;
using Strive.Core.Services.Chat.Requests;
using Strive.Core.Services.ConferenceControl;

namespace Strive.Core.Services.Chat
{
    public record ChatMessage(ChatMessageSender Sender, string Message, DateTimeOffset Timestamp,
        ChatMessageOptions Options);

    public record ChatMessageSender(string ParticipantId, ParticipantMetadata Meta);
}
