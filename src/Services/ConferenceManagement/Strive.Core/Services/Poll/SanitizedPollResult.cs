using System.Collections.Generic;

namespace Strive.Core.Services.Poll
{
    public record SanitizedPollResult(PollResults Results, IReadOnlyDictionary<string, string>? TokenIdToParticipant);
}
