namespace Strive.Core.Services.WhiteboardService
{
    public class WhiteboardOptions
    {
        public int MaxUndoHistory { get; set; } = 100;
        public int MaxUndoHistoryForParticipant { get; set; } = 20;
    }
}
