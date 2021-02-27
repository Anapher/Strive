using System;
using System.Collections.Generic;
using System.Linq;

namespace PaderConference.Core.Services.Chat.Channels
{
    public record PrivateChatChannel : ChatChannel
    {
        private readonly ISet<string> _participants;

        public PrivateChatChannel(ISet<string> participants)
        {
            if (participants.Count != 2)
                throw new ArgumentException("A private chat must have exactly 2 participants.", nameof(participants));

            _participants = participants;
        }

        public override ChatChannelType Type { get; } = ChatChannelType.Private;
        public IEnumerable<string> Participants => _participants.OrderBy(x => x);
    }
}
