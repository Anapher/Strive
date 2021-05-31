using Microsoft.Extensions.Logging;

namespace Strive.Core.Services.Poll.Types.Numeric
{
    public class NumericAnswerValidator : IPollAnswerValidator<NumericInstruction, NumericAnswer>
    {
        private readonly ILogger<NumericAnswerValidator> _logger;

        public NumericAnswerValidator(ILogger<NumericAnswerValidator> logger)
        {
            _logger = logger;
        }

        public bool Validate(NumericInstruction instruction, NumericAnswer answer)
        {
            if (instruction.Min > answer.Selected)
            {
                _logger.LogDebug(
                    "Answer rejected as the minimum is greater than the answer (min: {min}, selected: {selected})",
                    instruction.Min, answer.Selected);
                return false;
            }

            if (instruction.Max < answer.Selected)
            {
                _logger.LogDebug(
                    "Answer rejected as the maximum is smaller than the answer (max: {max}, selected: {selected})",
                    instruction.Max, answer.Selected);
                return false;
            }

            if (instruction.Step != null && answer.Selected % instruction.Step != 0)
            {
                _logger.LogDebug("Answer rejected as it does not match the step (step: {step}, selected: {selected})",
                    instruction.Step, answer.Selected);
                return false;
            }

            return true;
        }
    }
}
