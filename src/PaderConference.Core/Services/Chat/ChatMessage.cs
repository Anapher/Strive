using System;
using PaderConference.Core.Services.Chat.Dto;
using PaderConference.Core.Services.Chat.Filters;

namespace PaderConference.Core.Services.Chat
{
    public class ChatMessage
    {
        public ChatMessage(int messageId, ParticipantRef from, string message, IMessageFilter filter, SendingMode? mode,
            DateTimeOffset timestamp)
        {
            MessageId = messageId;
            From = from;
            Message = message;
            Filter = filter;
            Mode = mode;
            Timestamp = timestamp;
        }

        public int MessageId { get; set; }

        public ParticipantRef From { get; }

        public string Message { get; }

        public IMessageFilter Filter { get; }

        public SendingMode? Mode { get; }

        public DateTimeOffset Timestamp { get; }
    }
}