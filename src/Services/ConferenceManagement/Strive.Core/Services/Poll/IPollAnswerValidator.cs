namespace Strive.Core.Services.Poll
{
    public interface IPollAnswerValidator<in TInstruction, in TAnswer> where TAnswer : PollAnswer
        where TInstruction : PollInstruction<TAnswer>
    {
        bool Validate(TInstruction instruction, TAnswer answer);
    }
}
