using System;
using System.Linq;
using Strive.Core.Dto;

namespace Strive.Core.Services.Poll.Types.MultipleChoice
{
    public class MultipleChoiceAnswerValidator : IPollAnswerValidator<MultipleChoiceInstruction, MultipleChoiceAnswer>
    {
        public Error? Validate(MultipleChoiceInstruction instruction, MultipleChoiceAnswer answer)
        {
            if (answer.Selected.Length == 0)
            {
                return PollError.AnswerValidationFailed("Answer rejected because no options were selected");
            }

            if (answer.Selected.Length > instruction.MaxSelections)
            {
                return PollError.AnswerValidationFailed(
                    $"Multiple choice answer rejected because more options than allowed were selected ({answer.Selected.Length} selected, {instruction.MaxSelections} allowed)");
            }

            if (answer.Selected.Any(x => !instruction.Options.Contains(x, StringComparer.Ordinal)))
            {
                return PollError.AnswerValidationFailed(
                    "Multiple choice answer rejected as an invalid option was selected");
            }

            return null;
        }
    }
}
