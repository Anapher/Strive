namespace PaderConference.Core.Services.BreakoutRoom
{
    public record BreakoutRoomSyncObject
    {
        public ActiveBreakoutRoomState? Active { get; init; }
    }
}
