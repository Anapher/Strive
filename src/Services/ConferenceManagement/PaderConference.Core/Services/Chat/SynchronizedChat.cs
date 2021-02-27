using System.Collections.Immutable;

namespace PaderConference.Core.Services.Chat
{
    public record SynchronizedChat(IImmutableDictionary<string, bool> ParticipantsTyping);
}
