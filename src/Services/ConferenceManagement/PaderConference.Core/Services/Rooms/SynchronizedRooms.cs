using System.Collections.Generic;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Rooms
{
    public record SynchronizedRooms(IReadOnlyList<Room> Rooms, string DefaultRoomId,
        IReadOnlyDictionary<string, string> Participants)
    {
        public static SynchronizedObjectId SyncObjId { get; } = new(SynchronizedObjectIds.ROOMS);
    }
}
