using Strive.Core.Dto;

namespace Strive.Core.Services.Poll.Types.Numeric
{
    public class NumericAnswerValidator : IPollAnswerValidator<NumericInstruction, NumericAnswer>
    {
        public Error? Validate(NumericInstruction instruction, NumericAnswer answer)
        {
            if (instruction.Min > answer.Selected)
            {
                return PollError.AnswerValidationFailed(
                    $"Answer rejected as the minimum is greater than the answer (min: {instruction.Min}, selected: {answer.Selected})");
            }

            if (instruction.Max < answer.Selected)
            {
                return PollError.AnswerValidationFailed(
                    $"Answer rejected as the maximum is smaller than the answer (max: {instruction.Max}, selected: {answer.Selected})");
            }

            if (instruction.Step != null && answer.Selected % instruction.Step != 0)
            {
                return PollError.AnswerValidationFailed(
                    $"Answer rejected as it does not match the step (step: {instruction.Step}, selected: {answer.Selected})");
            }

            return null;
        }
    }
}
