using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Strive.Core.Services.Poll.Types.MultipleChoice
{
    public class MultipleChoiceAggregator : SelectionPollAggregator,
        IPollAnswerAggregator<MultipleChoiceInstruction, MultipleChoiceAnswer>
    {
        public async ValueTask<PollResults> Aggregate(MultipleChoiceInstruction instruction,
            IReadOnlyDictionary<string, MultipleChoiceAnswer> answers)
        {
            return Aggregate(instruction.Options,
                answers.Select(x => new KeyValuePair<string, string[]>(x.Key, x.Value.Selected)));
        }
    }
}
