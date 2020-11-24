namespace PaderConference.Core.Services.BreakoutRoom
{
    public class BreakoutRoomSyncObject
    {
        public BreakoutRoomSyncObject(ActiveBreakoutRoomState? active)
        {
            Active = active;
        }

        public ActiveBreakoutRoomState? Active { get; }
    }
}
