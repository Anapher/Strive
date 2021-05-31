using System.Collections.Generic;

namespace Strive.Core.Services.Poll.Types.Numeric
{
    public record NumericPollResults(IReadOnlyDictionary<string, decimal> Answers) : PollResults;
}
