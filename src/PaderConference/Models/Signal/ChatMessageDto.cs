#pragma warning disable 8618
using System;

namespace PaderConference.Models.Signal
{
    public class ChatMessageDto
    {
        public int MessageId { get; set; }

        public string ParticipantId { get; set; }

        public string Message { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}