using System;
using System.Collections.Generic;
using PaderConference.Core.Services.Chat.Channels;

namespace PaderConference.Core.Services.Chat
{
    public interface IParticipantTypingTimer
    {
        void RemoveParticipantTypingAfter(Participant participant, ChatChannel channel, TimeSpan timespan);

        IEnumerable<ChatChannel> CancelAllTimers(Participant participant);

        void CancelTimer(Participant participant, ChatChannel channel);
    }
}
