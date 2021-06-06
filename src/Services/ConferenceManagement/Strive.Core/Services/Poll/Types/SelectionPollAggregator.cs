using System.Collections.Generic;
using System.Linq;

namespace Strive.Core.Services.Poll.Types
{
    public abstract class SelectionPollAggregator
    {
        public SelectionPollResults Aggregate(string[] options, IEnumerable<KeyValuePair<string, string[]>> answers)
        {
            var result = options.ToDictionary(option => option,
                option => (IReadOnlyList<string>) answers.Where(x => x.Value.Contains(option)).Select(x => x.Key)
                    .OrderBy(x => x).ToList());

            return new SelectionPollResults(result);
        }
    }
}
