﻿#pragma warning disable 8618
namespace PaderConference.Infrastructure.Services.Media.Communication
{
    public class ClientDisconnectedMessage
    {
        public string ConnectionId { get; set; }

        public string ParticipantId { get; set; }
    }
}