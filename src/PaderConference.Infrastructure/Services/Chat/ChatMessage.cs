using System;
using PaderConference.Infrastructure.Services.Chat.Filters;

namespace PaderConference.Infrastructure.Services.Chat
{
    public class ChatMessage
    {
        public ChatMessage(int messageId, string participantId, string message, IMessageFilter filter,
            DateTimeOffset timestamp)
        {
            MessageId = messageId;
            ParticipantId = participantId;
            Message = message;
            Filter = filter;
            Timestamp = timestamp;
        }

        public int MessageId { get; set; }

        public string ParticipantId { get; }

        public string Message { get; }

        public IMessageFilter Filter { get; }

        public DateTimeOffset Timestamp { get; }
    }
}