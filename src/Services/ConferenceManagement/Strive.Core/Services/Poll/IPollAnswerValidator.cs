using Strive.Core.Dto;

namespace Strive.Core.Services.Poll
{
    public interface IPollAnswerValidator<in TInstruction, in TAnswer> where TAnswer : PollAnswer
        where TInstruction : PollInstruction<TAnswer>
    {
        Error? Validate(TInstruction instruction, TAnswer answer);
    }
}
