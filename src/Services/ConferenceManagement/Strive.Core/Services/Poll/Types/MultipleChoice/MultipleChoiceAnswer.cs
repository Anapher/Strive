namespace Strive.Core.Services.Poll.Types.MultipleChoice
{
    public record MultipleChoiceAnswer(string[] Selected) : PollAnswer;
}
