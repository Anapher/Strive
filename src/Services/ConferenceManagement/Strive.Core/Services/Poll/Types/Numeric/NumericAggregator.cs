using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Strive.Core.Services.Poll.Types.Numeric
{
    public class NumericAggregator : IPollAnswerAggregator<NumericInstruction, NumericAnswer>
    {
        public ValueTask<PollResults> Aggregate(NumericInstruction instruction,
            IReadOnlyDictionary<string, NumericAnswer> answers)
        {
            return new(new NumericPollResults(answers.ToDictionary(x => x.Key, x => x.Value.Selected)));
        }
    }
}
