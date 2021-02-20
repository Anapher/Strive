using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Gateways.Repositories;

namespace PaderConference.Core.Services.Rooms.Gateways
{
    public interface IRoomRepository
    {
        Task CreateRoom(string conferenceId, Room room);

        /// <exception cref="ConcurrencyException">A concurrency exception occurs if the room does not exist</exception>
        Task SetParticipantRoom(Participant participant, string roomId);

        Task UnsetParticipantRoom(Participant participant);

        /// <returns>Returns true if the room was actually removed, false if the room did not exist</returns>
        Task<bool> RemoveRoom(string conferenceId, string roomId);

        Task<IReadOnlyList<Participant>> GetParticipantsOfRoom(string conferenceId, string roomId);

        Task<IEnumerable<Room>> GetRooms(string conferenceId);

        Task<IReadOnlyDictionary<string, string>> GetParticipantRooms(string conferenceId);

        Task<DeleteAllResult> DeleteAllRoomsAndMappingsOfConference(string conferenceId);
    }

    public struct DeleteAllResult
    {
        public IReadOnlyList<string> DeletedParticipants { get; set; }

        public IReadOnlyList<string> DeletedRooms { get; set; }
    }
}
