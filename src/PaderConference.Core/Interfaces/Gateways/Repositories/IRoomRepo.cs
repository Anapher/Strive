using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Services.Rooms;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public interface IRoomRepo
    {
        /// <summary>
        ///     Create a room in the conference
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <param name="room">The room that should be created</param>
        Task CreateRoom(string conferenceId, Room room);

        /// <summary>
        ///     Delete rooms from a conference
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <param name="roomIds">The ids of the rooms that should be removed</param>
        Task DeleteRooms(string conferenceId, IEnumerable<string> roomIds);

        /// <summary>
        ///     Get all rooms of a conference
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <returns>Returns a dictionary of the room id as key and <see cref="Room" /> as value</returns>
        Task<Dictionary<string, Room>> GetAll(string conferenceId);

        /// <summary>
        ///     Get information of a room in a conference
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <param name="roomId">The id of the room</param>
        /// <returns>Return the room or null if not found</returns>
        Task<Room?> Get(string conferenceId, string roomId);

        /// <summary>
        ///     Delete all rooms of a conference
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        Task DeleteAll(string conferenceId);

        /// <summary>
        ///     Get a mapping of participant ids to rooms
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <returns>Return a dictionary with participant ids as keys and the room id as values</returns>
        Task<Dictionary<string, string>> GetParticipantRooms(string conferenceId);

        /// <summary>
        ///     Set the room id of a participant
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <param name="participantId">The participant id</param>
        /// <param name="roomId">The new room id</param>
        Task SetParticipantRoom(string conferenceId, string participantId, string roomId);

        /// <summary>
        ///     Unset the room id of a participant (delete the mapping entry)
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <param name="participantId">The participant id</param>
        Task UnsetParticipantRoom(string conferenceId, string participantId);

        /// <summary>
        ///     Get the room of a participant if possible, else return null
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <param name="participantId">The participant id</param>
        /// <returns>Return the room id of the participant or null if the participant is not mapped to a room.</returns>
        Task<string?> GetParticipantRoom(string conferenceId, string participantId);

        /// <summary>
        ///     Delete the whole participant mapping, so all participants will be unset
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        Task DeleteParticipantMapping(string conferenceId);

        /// <summary>
        ///     Get the default room id
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <returns>Return the default room id or null if not found.</returns>
        Task<string?> GetDefaultRoomId(string conferenceId);

        /// <summary>
        ///     Get the permissions of a specific room
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <param name="roomId">The room id</param>
        /// <returns>The permissions set for the specified room.</returns>
        Task<Dictionary<string, JValue>> GetRoomPermissions(string conferenceId, string roomId);
    }
}
