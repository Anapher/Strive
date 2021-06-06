using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Strive.Core.Services.Poll.Types.SingleChoice
{
    public class SingleChoiceAggregator : SelectionPollAggregator,
        IPollAnswerAggregator<SingleChoiceInstruction, SingleChoiceAnswer>
    {
        public async ValueTask<PollResults> Aggregate(SingleChoiceInstruction instruction,
            IReadOnlyDictionary<string, SingleChoiceAnswer> answers)
        {
            return Aggregate(instruction.Options,
                answers.Select(x => new KeyValuePair<string, string[]>(x.Key, new[] {x.Value.Selected})));
        }
    }
}
