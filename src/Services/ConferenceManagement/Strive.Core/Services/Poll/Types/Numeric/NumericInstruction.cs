namespace Strive.Core.Services.Poll.Types.Numeric
{
    public record NumericInstruction(double Min, double Max, double Step) : PollInstruction;
}
