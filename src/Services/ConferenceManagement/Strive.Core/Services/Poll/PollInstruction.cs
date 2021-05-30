namespace Strive.Core.Services.Poll
{
    public abstract record PollInstruction;

    public abstract record PollInstruction<T> : PollInstruction where T : PollAnswer;
}
