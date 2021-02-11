using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Infrastructure.Redis.Scripts;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class RoomRepository : IRoomRepository, IRedisRepo
    {
        private const string ROOMS_PROPERTY_KEY = "Rooms";
        private const string ROOMMAPPING_PROPERTY_KEY = "RoomMapping";

        private readonly IRedisDatabase _redisDatabase;

        public RoomRepository(IRedisDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        public async Task CreateRoom(string conferenceId, Room room)
        {
            var key = GetRoomListKey(conferenceId);
            await _redisDatabase.HashSetAsync(key, room.RoomId, room);
        }

        public async Task<bool> RemoveRoom(string conferenceId, string roomId)
        {
            var key = GetRoomListKey(conferenceId);
            return await _redisDatabase.HashDeleteAsync(key, roomId);
        }

        public async Task<IReadOnlyList<string>> GetParticipantsOfRoom(string conferenceId, string roomId)
        {
            var key = GetRoomMappingKey(conferenceId);
            var hashValues = await _redisDatabase.Database.HashGetAllAsync(key);

            return hashValues.Where(x => x.Name == roomId).Select(x => (string) x.Value).ToList();
        }

        public async Task<IEnumerable<Room>> GetRooms(string conferenceId)
        {
            var key = GetRoomListKey(conferenceId);
            var result = await _redisDatabase.HashGetAllAsync<Room>(key);
            return result.Values;
        }

        public async Task<IReadOnlyDictionary<string, string>> GetParticipantRooms(string conferenceId)
        {
            var key = GetRoomMappingKey(conferenceId);
            var result = await _redisDatabase.Database.HashGetAllAsync(key);
            return result.ToDictionary(x => (string) x.Name, x => (string) x.Value);
        }

        public async Task<DeleteAllResult> DeleteAllRoomsAndMappingsOfConference(string conferenceId)
        {
            var mappingKey = GetRoomMappingKey(conferenceId);
            var roomListKey = GetRoomListKey(conferenceId);

            var trans = _redisDatabase.Database.CreateTransaction();
            var allParticipantsTask = trans.HashGetAllAsync(mappingKey);
            var allRoomsTask = trans.HashGetAllAsync(roomListKey);

            _ = trans.KeyDeleteAsync(mappingKey);
            _ = trans.KeyDeleteAsync(roomListKey);

            await trans.ExecuteAsync();

            var allParticipants = await allParticipantsTask;
            var allRooms = await allRoomsTask;

            return new DeleteAllResult
            {
                DeletedParticipants = allParticipants.Select(x => (string) x.Name).ToList(),
                DeletedRooms = allRooms.Select(x => (string) x.Name).ToList(),
            };
        }

        public async Task SetParticipantRoom(string conferenceId, string participantId, string roomId)
        {
            var roomMappingKey = GetRoomMappingKey(conferenceId);
            var roomListKey = GetRoomListKey(conferenceId);

            var scriptContent = RedisScriptLoader.Load(RedisScript.RoomRepository_SetParticipantRoom);
            var result = await _redisDatabase.Database.ScriptEvaluateAsync(scriptContent,
                new RedisKey[] {roomMappingKey, roomListKey, participantId, roomId});

            if (!(bool) result)
                throw new ConcurrencyException("Failed to set room of participant: The room does not exist.");
        }

        public async Task UnsetParticipantRoom(string conferenceId, string participantId)
        {
            var roomMappingKey = GetRoomMappingKey(conferenceId);
            await _redisDatabase.Database.HashDeleteAsync(roomMappingKey, participantId);
        }

        private string GetRoomMappingKey(string conferenceId)
        {
            return RedisKeyBuilder.ForProperty(ROOMMAPPING_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }

        private string GetRoomListKey(string conferenceId)
        {
            return RedisKeyBuilder.ForProperty(ROOMS_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
