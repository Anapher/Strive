using System.Collections.Immutable;

namespace PaderConference.Core.Services.Chat
{
    public record ChatSynchronizedObject
    {
        public IImmutableList<string> ParticipantsTyping { get; init; } = ImmutableList<string>.Empty;
    }
}