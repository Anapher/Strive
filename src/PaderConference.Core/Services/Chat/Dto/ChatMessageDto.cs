#pragma warning disable 8618
using System;
using PaderConference.Core.Services.Chat.Requests;

namespace PaderConference.Core.Services.Chat.Dto
{
    public class ChatMessageDto
    {
        public int MessageId { get; set; }

        public ParticipantRef? From { get; set; }

        public string Message { get; set; }

        public SendingMode? Mode { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}