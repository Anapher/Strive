using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Strive.Core.Services.Poll.Types.MultipleChoice
{
    public class MultipleChoiceAggregator : IPollAnswerAggregator<MultipleChoiceInstruction, MultipleChoiceAnswer>
    {
        public async ValueTask<PollResults> Aggregate(MultipleChoiceInstruction instruction,
            IReadOnlyDictionary<string, MultipleChoiceAnswer> answers)
        {
            var result = instruction.Options.ToDictionary(option => option,
                option => (IReadOnlyList<string>) answers.Where(x => x.Value.Selected.Contains(option))
                    .Select(x => x.Key).OrderBy(x => x).ToList());

            return new MultipleChoicePollResults(result);
        }
    }
}
