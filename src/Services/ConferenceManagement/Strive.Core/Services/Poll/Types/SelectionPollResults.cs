using System.Collections.Generic;

namespace Strive.Core.Services.Poll.Types
{
    public record SelectionPollResults(IReadOnlyDictionary<string, IReadOnlyList<string>> Options) : PollResults;
}
