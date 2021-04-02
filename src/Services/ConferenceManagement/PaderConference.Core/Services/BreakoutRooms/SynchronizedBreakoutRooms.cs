using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.BreakoutRooms
{
    public record SynchronizedBreakoutRooms(BreakoutRoomsConfig? Active)
    {
        public static SynchronizedObjectId SyncObjId { get; } = new(SynchronizedObjectIds.BREAKOUT_ROOMS);
    }
}
