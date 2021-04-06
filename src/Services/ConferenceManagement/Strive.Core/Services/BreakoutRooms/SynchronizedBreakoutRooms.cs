using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.BreakoutRooms
{
    public record SynchronizedBreakoutRooms(BreakoutRoomsConfig? Active)
    {
        public static SynchronizedObjectId SyncObjId { get; } = new(SynchronizedObjectIds.BREAKOUT_ROOMS);
    }
}
