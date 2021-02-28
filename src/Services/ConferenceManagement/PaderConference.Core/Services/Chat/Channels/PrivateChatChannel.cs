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

        public IEnumerable<string> Participants => _participants.OrderBy(x => x);

        public virtual bool Equals(PrivateChatChannel? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && _participants.SetEquals(other._participants);
        }

        public override int GetHashCode()
        {
            var ordered = _participants.OrderBy(x => x).ToList();
            return HashCode.Combine(ordered[0], ordered[1]);
        }
    }
}
