using System.Collections.Immutable;

namespace PaderConference.Core.Services.Chat
{
    public class ChatSynchronizedObject
    {
        public ChatSynchronizedObject(IImmutableList<string> participantsTyping)
        {
            ParticipantsTyping = participantsTyping;
        }

        public IImmutableList<string> ParticipantsTyping { get; }
    }
}