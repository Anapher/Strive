using System.Collections.Generic;

namespace Strive.Core.Services.Poll
{
    public record SanitizedPollResult(PollResults Results, int ParticipantsAnswered,
        IReadOnlyDictionary<string, string>? TokenIdToParticipant);
}
