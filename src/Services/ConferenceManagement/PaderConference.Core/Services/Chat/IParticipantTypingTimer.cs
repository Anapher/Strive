using System;
using System.Collections.Generic;

namespace PaderConference.Core.Services.Chat
{
    public interface IParticipantTypingTimer
    {
        void RemoveParticipantTypingAfter(Participant participant, string channel, TimeSpan timespan);

        IEnumerable<string> CancelAllTimers(Participant participant);

        void CancelTimer(Participant participant, string channel);
    }
}
