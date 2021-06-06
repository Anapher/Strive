using System;
using System.Linq;
using Strive.Core.Dto;

namespace Strive.Core.Services.Poll.Types.SingleChoice
{
    public class SingleChoiceAnswerValidator : IPollAnswerValidator<SingleChoiceInstruction, SingleChoiceAnswer>
    {
        public Error? Validate(SingleChoiceInstruction instruction, SingleChoiceAnswer answer)
        {
            if (!instruction.Options.Contains(answer.Selected, StringComparer.Ordinal))
            {
                return PollError.AnswerValidationFailed("The selected option of the answer does not exist");
            }

            return null;
        }
    }
}
