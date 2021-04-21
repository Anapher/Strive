using System.Collections.Generic;
using System.Linq;
using Strive.Core.Services.Rooms;

namespace Strive.Core.Services.Scenes.Utilities
{
    public static class SceneUtilities
    {
        public static bool ParticipantsOfRoomChanged(string roomId, SynchronizedRooms rooms,
            SynchronizedRooms? previousRooms)
        {
            if (previousRooms == null)
                return true;

            var roomsParticipants = GetParticipantsOfRoom(rooms, roomId);
            var previousRoomsParticipants = GetParticipantsOfRoom(previousRooms, roomId);

            return !roomsParticipants.SequenceEqual(previousRoomsParticipants);
        }

        public static IEnumerable<string> GetParticipantsOfRoom(SynchronizedRooms rooms, string roomId)
        {
            return rooms.Participants.Where(x => x.Value == roomId).Select(x => x.Key);
        }
    }
}
