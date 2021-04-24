using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Extensions;
using Strive.Core.Interfaces.Gateways.Repositories;
using Strive.Core.Services;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Extensions;
using Strive.Infrastructure.KeyValue.Redis.Scripts;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class RoomRepository : IRoomRepository, IKeyValueRepo
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

            return hashValues.Where(x => x.Value == roomId).Select(x => new Participant(conferenceId, x.Key)).ToList();
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
                var mappingTask = trans.HashGetAllAsync(mappingKey);
                var allRoomsTask = trans.HashGetAllAsync(roomListKey);

                _ = trans.KeyDeleteAsync(mappingKey);
                _ = trans.KeyDeleteAsync(roomListKey);

                await trans.ExecuteAsync();

                var mapping = await mappingTask;
                var allRooms = await allRoomsTask;

                return new DeleteAllResult(mapping, allRooms.Select(x => x.Key).ToList());
            }
        }

        public async ValueTask<string?> GetRoomOfParticipant(Participant participant)
        {
            var mappingKey = GetRoomMappingKey(participant.ConferenceId);
            return await _database.HashGetAsync(mappingKey, participant.Id);
        }

        public async ValueTask<string?> SetParticipantRoom(Participant participant, string roomId)
        {
            var roomMappingKey = GetRoomMappingKey(participant.ConferenceId);
            var roomListKey = GetRoomListKey(participant.ConferenceId);

            using var transaction = _database.CreateTransaction();

            var previousRoomTask = transaction.HashGetAsync(roomMappingKey, participant.Id);

            var scriptTask = transaction.ExecuteScriptAsync(RedisScript.RoomRepository_SetParticipantRoom,
                roomMappingKey, roomListKey, participant.Id, roomId);

            await transaction.ExecuteAsync();

            if (!(bool) await scriptTask)
                throw new ConcurrencyException("Failed to set room of participant: The room does not exist.");

            return await previousRoomTask;
        }

        public async ValueTask<string?> UnsetParticipantRoom(Participant participant)
        {
            var roomMappingKey = GetRoomMappingKey(participant.ConferenceId);

            using var transaction = _database.CreateTransaction();

            var previousRoomTask = transaction.HashGetAsync(roomMappingKey, participant.Id);
            _ = transaction.HashDeleteAsync(roomMappingKey, participant.Id);

            await transaction.ExecuteAsync();

            return await previousRoomTask;
        }

        private static string GetRoomMappingKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(ROOMMAPPING_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }

        private static string GetRoomListKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(ROOMS_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
