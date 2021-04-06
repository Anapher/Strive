using System.Collections.Generic;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Rooms
{
    public record SynchronizedRooms(IReadOnlyList<Room> Rooms, string DefaultRoomId,
        IReadOnlyDictionary<string, string> Participants)
    {
        public static SynchronizedObjectId SyncObjId { get; } = new(SynchronizedObjectIds.ROOMS);
    }
}
