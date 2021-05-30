using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Strive.Core.Services.Poll.Types.SingleChoice
{
    public class SingleChoiceAnswerValidator : IPollAnswerValidator<SingleChoiceInstruction, SingleChoiceAnswer>
    {
        private readonly ILogger<SingleChoiceAnswerValidator> _logger;

        public SingleChoiceAnswerValidator(ILogger<SingleChoiceAnswerValidator> logger)
        {
            _logger = logger;
        }

        public bool Validate(SingleChoiceInstruction instruction, SingleChoiceAnswer answer)
        {
            if (!instruction.Options.Contains(answer.Selected, StringComparer.Ordinal))
            {
                _logger.LogDebug("Answer does not exist");
                return false;
            }

            return true;
        }
    }
}
