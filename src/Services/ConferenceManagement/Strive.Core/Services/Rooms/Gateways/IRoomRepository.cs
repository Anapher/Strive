using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Interfaces.Gateways.Repositories;

namespace Strive.Core.Services.Rooms.Gateways
{
    public interface IRoomRepository
    {
        ValueTask CreateRoom(string conferenceId, Room room);

        /// <exception cref="ConcurrencyException">A concurrency exception occurs if the room does not exist</exception>
        /// <returns>Returns previous room id</returns>
        ValueTask<string?> SetParticipantRoom(Participant participant, string roomId);

        /// <returns>Returns previous room id</returns>
        ValueTask<string?> UnsetParticipantRoom(Participant participant);

        /// <returns>Returns true if the room was actually removed, false if the room did not exist</returns>
        ValueTask<bool> RemoveRoom(string conferenceId, string roomId);

        ValueTask<IReadOnlyList<Participant>> GetParticipantsOfRoom(string conferenceId, string roomId);

        ValueTask<IEnumerable<Room>> GetRooms(string conferenceId);

        ValueTask<IReadOnlyDictionary<string, string>> GetParticipantRooms(string conferenceId);

        ValueTask<DeleteAllResult> DeleteAllRoomsAndMappingsOfConference(string conferenceId);

        ValueTask<string?> GetRoomOfParticipant(Participant participant);
    }

    public readonly struct DeleteAllResult
    {
        public DeleteAllResult(IReadOnlyDictionary<string, string> deletedMapping, IReadOnlyList<string> deletedRooms)
        {
            DeletedMapping = deletedMapping;
            DeletedRooms = deletedRooms;
        }

        public IReadOnlyDictionary<string, string> DeletedMapping { get; }

        public IReadOnlyList<string> DeletedRooms { get; }
    }
}
