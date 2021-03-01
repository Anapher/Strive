using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Extensions;
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

        public async ValueTask CreateRoom(string conferenceId, Room room)
        {
            var key = GetRoomListKey(conferenceId);
            await _database.HashSetAsync(key, room.RoomId, room);
        }

        public async ValueTask<bool> RemoveRoom(string conferenceId, string roomId)
        {
            var key = GetRoomListKey(conferenceId);
            return await _database.HashDeleteAsync(key, roomId);
        }

        public async ValueTask<IReadOnlyList<Participant>> GetParticipantsOfRoom(string conferenceId, string roomId)
        {
            var key = GetRoomMappingKey(conferenceId);
            var hashValues = await _database.HashGetAllAsync(key);

            return hashValues.Where(x => x.Key == roomId).Select(x => new Participant(conferenceId, x.Value)).ToList();
        }

        public async ValueTask<IEnumerable<Room>> GetRooms(string conferenceId)
        {
            var key = GetRoomListKey(conferenceId);
            var result = await _database.HashGetAllAsync<Room>(key);
            return result.Values.WhereNotNull();
        }

        public async ValueTask<IReadOnlyDictionary<string, string>> GetParticipantRooms(string conferenceId)
        {
            var key = GetRoomMappingKey(conferenceId);
            return await _database.HashGetAllAsync(key);
        }

        public async ValueTask<DeleteAllResult> DeleteAllRoomsAndMappingsOfConference(string conferenceId)
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

        public async ValueTask<string?> GetRoomOfParticipant(Participant participant)
        {
            var mappingKey = GetRoomMappingKey(participant.ConferenceId);
            return await _database.HashGetAsync(mappingKey, participant.Id);
        }

        public async ValueTask SetParticipantRoom(Participant participant, string roomId)
        {
            var roomMappingKey = GetRoomMappingKey(participant.ConferenceId);
            var roomListKey = GetRoomListKey(participant.ConferenceId);

            var result = await _database.ExecuteScriptAsync(RedisScript.RoomRepository_SetParticipantRoom,
                roomMappingKey, roomListKey, participant.Id, roomId);

            if (!(bool) result)
                throw new ConcurrencyException("Failed to set room of participant: The room does not exist.");
        }

        public async ValueTask UnsetParticipantRoom(Participant participant)
        {
            var roomMappingKey = GetRoomMappingKey(participant.ConferenceId);
            await _database.HashDeleteAsync(roomMappingKey, participant.Id);
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
