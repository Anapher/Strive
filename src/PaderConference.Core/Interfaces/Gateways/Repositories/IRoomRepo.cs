using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using PaderConference.Core.Services.Rooms;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public interface IRoomRepo
    {
        Task CreateRoom(string conferenceId, Room room);

        Task DeleteRooms(string conferenceId, IEnumerable<string> roomIds);

        Task<Dictionary<string, Room>> GetAll(string conferenceId);

        Task<Room> Get(string conferenceId, string roomId);

        Task DeleteAll(string conferenceId);


        Task<Dictionary<string, string>> GetParticipantRooms(string conferenceId);

        Task SetParticipantRoom(string conferenceId, string participantId, string roomId);

        Task UnsetParticipantRoom(string conferenceId, string participantId);

        Task<string?> GetParticipantRoom(string conferenceId, string participantId);

        Task DeleteParticipantToRoomMap(string conferenceId);


        Task<string?> GetDefaultRoomId(string conferenceId);
        Task<Dictionary<string, JsonElement>> GetRoomPermissions(string conferenceId, string roomId);
    }
}
