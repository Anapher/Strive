using System.Collections.Generic;

namespace Strive.Core.Services.Poll.Types.TagCloud
{
    public record TagCloudResults(IReadOnlyDictionary<string, IReadOnlyList<string>> Tags) : PollResults;
}
