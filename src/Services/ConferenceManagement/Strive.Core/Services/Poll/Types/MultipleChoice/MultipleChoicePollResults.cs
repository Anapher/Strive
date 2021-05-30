using System.Collections.Generic;

namespace Strive.Core.Services.Poll.Types.MultipleChoice
{
    public record MultipleChoicePollResults(IReadOnlyDictionary<string, IReadOnlyList<string>> Options) : PollResults;
}
