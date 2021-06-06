using System.Collections.Generic;
using System.Threading.Tasks;

namespace Strive.Core.Services.Poll
{
    public interface IPollAnswerAggregator<in TInstruction, TAnswer> where TAnswer : PollAnswer
        where TInstruction : PollInstruction<TAnswer>
    {
        ValueTask<PollResults> Aggregate(TInstruction instruction, IReadOnlyDictionary<string, TAnswer> answers);
    }
}
