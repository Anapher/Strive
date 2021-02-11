using System.Collections.Generic;

namespace PaderConference.Core.Services.Rooms
{
    public record SynchronizedRooms(IReadOnlyList<Room> Rooms, string DefaultRoomId,
        IReadOnlyDictionary<string, string> Participants);
}
