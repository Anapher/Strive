namespace Strive.Core.Services.Poll
{
    public record PollState(bool IsOpen, bool ResultsPublished)
    {
        public static readonly PollState Default = new(false, false);
    }
}
