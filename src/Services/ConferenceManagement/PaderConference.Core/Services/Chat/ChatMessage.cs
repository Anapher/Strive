using System;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Core.Services.ConferenceControl;

namespace PaderConference.Core.Services.Chat
{
    public record ChatMessage(ChatMessageSender Sender, string Message, DateTimeOffset Timestamp,
        ChatMessageOptions Options);

    public record ChatMessageSender(string ParticipantId, ParticipantMetadata DisplayName);
}
