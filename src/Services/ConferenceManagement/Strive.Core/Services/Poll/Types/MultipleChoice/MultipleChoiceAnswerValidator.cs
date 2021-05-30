using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Strive.Core.Services.Poll.Types.MultipleChoice
{
    public class MultipleChoiceAnswerValidator : IPollAnswerValidator<MultipleChoiceInstruction, MultipleChoiceAnswer>
    {
        private readonly ILogger<MultipleChoiceAnswerValidator> _logger;

        public MultipleChoiceAnswerValidator(ILogger<MultipleChoiceAnswerValidator> logger)
        {
            _logger = logger;
        }

        public bool Validate(MultipleChoiceInstruction instruction, MultipleChoiceAnswer answer)
        {
            if (answer.Selected.Length == 0)
            {
                _logger.LogDebug("Multiple choice answer rejected because no options were selected");
                return false;
            }

            if (answer.Selected.Length > instruction.MaxSelections)
            {
                _logger.LogDebug(
                    "Multiple choice answer rejected because more options than allowed were selected ({selected} selected, {allowed} allowed)",
                    answer.Selected.Length, instruction.MaxSelections);

                return false;
            }

            if (answer.Selected.Any(x => !instruction.Options.Contains(x, StringComparer.Ordinal)))
            {
                _logger.LogDebug("Multiple choice answer rejected as an invalid option was selected");
                return false;
            }

            return true;
        }
    }
}
