using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Scripts;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class RoomRepository : IRoomRepository, IRedisRepo
    {
        private const string ROOMS_PROPERTY_KEY = "Rooms";
        private const string ROOMMAPPING_PROPERTY_KEY = "RoomMapping";

        private readonly IKeyValueDatabase _database;

        public RoomRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async Task CreateRoom(string conferenceId, Room room)
        {
            var key = GetRoomListKey(conferenceId);
            await _database.HashSetAsync(key, room.RoomId, room);
        }

        public async Task<bool> RemoveRoom(string conferenceId, string roomId)
        {
            var key = GetRoomListKey(conferenceId);
            return await _database.HashDeleteAsync(key, roomId);
        }

        public async Task<IReadOnlyList<string>> GetParticipantsOfRoom(string conferenceId, string roomId)
        {
            var key = GetRoomMappingKey(conferenceId);
            var hashValues = await _database.HashGetAllAsync(key);

            return hashValues.Where(x => x.Key == roomId).Select(x => x.Value).ToList();
        }

        public async Task<IEnumerable<Room>> GetRooms(string conferenceId)
        {
            var key = GetRoomListKey(conferenceId);
            var result = await _database.HashGetAllAsync<Room>(key);
            return result.Values;
        }

        public async Task<IReadOnlyDictionary<string, string>> GetParticipantRooms(string conferenceId)
        {
            var key = GetRoomMappingKey(conferenceId);
            var result = await _database.HashGetAllAsync(key);
            return result.ToDictionary(x => x.Key, x => x.Value);
        }

        public async Task<DeleteAllResult> DeleteAllRoomsAndMappingsOfConference(string conferenceId)
        {
            var mappingKey = GetRoomMappingKey(conferenceId);
            var roomListKey = GetRoomListKey(conferenceId);

            using (var trans = _database.CreateTransaction())
            {
                var allParticipantsTask = trans.HashGetAllAsync(mappingKey);
                var allRoomsTask = trans.HashGetAllAsync(roomListKey);

                _ = trans.KeyDeleteAsync(mappingKey);
                _ = trans.KeyDeleteAsync(roomListKey);

                await trans.ExecuteAsync();

                var allParticipants = await allParticipantsTask;
                var allRooms = await allRoomsTask;

                return new DeleteAllResult
                {
                    DeletedParticipants = allParticipants.Select(x => x.Key).ToList(),
                    DeletedRooms = allRooms.Select(x => x.Key).ToList(),
                };
            }
        }

        public async Task SetParticipantRoom(string conferenceId, string participantId, string roomId)
        {
            var roomMappingKey = GetRoomMappingKey(conferenceId);
            var roomListKey = GetRoomListKey(conferenceId);

            var result = await _database.ExecuteScriptAsync(RedisScript.RoomRepository_SetParticipantRoom,
                roomMappingKey, roomListKey, participantId, roomId);

            if (!(bool) result)
                throw new ConcurrencyException("Failed to set room of participant: The room does not exist.");
        }

        public async Task UnsetParticipantRoom(string conferenceId, string participantId)
        {
            var roomMappingKey = GetRoomMappingKey(conferenceId);
            await _database.HashDeleteAsync(roomMappingKey, participantId);
        }

        private static string GetRoomMappingKey(string conferenceId)
        {
            return RedisKeyBuilder.ForProperty(ROOMMAPPING_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }

        private static string GetRoomListKey(string conferenceId)
        {
            return RedisKeyBuilder.ForProperty(ROOMS_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
