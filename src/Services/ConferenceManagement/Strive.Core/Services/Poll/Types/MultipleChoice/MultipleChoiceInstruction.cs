namespace Strive.Core.Services.Poll.Types.MultipleChoice
{
    public record MultipleChoiceInstruction
        (string[] Options, int? MaxSelections) : PollInstruction<MultipleChoiceAnswer>;
}
