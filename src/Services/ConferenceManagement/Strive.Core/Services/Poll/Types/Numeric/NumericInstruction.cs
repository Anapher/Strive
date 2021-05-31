namespace Strive.Core.Services.Poll.Types.Numeric
{
    public record NumericInstruction(decimal? Min, decimal? Max, decimal? Step) : PollInstruction<NumericAnswer>;
}
