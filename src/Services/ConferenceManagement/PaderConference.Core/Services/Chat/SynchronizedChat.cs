using System.Collections.Generic;

namespace PaderConference.Core.Services.Chat
{
    public record SynchronizedChat(IReadOnlyDictionary<string, bool> ParticipantsTyping);
}
