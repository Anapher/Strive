#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.

using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Rooms;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class RoomRepo : IRoomRepo, IRedisRepo
    {
        private readonly IRedisDatabase _database;

        public RoomRepo(IRedisDatabase database)
        {
            _database = database;
        }

        public Task CreateRoom(string conferenceId, Room room)
        {
            return _database.HashSetAsync(RedisKeys.Rooms.RoomList(conferenceId), room.RoomId, room);
        }

        public Task DeleteRooms(string conferenceId, IEnumerable<string> roomIds)
        {
            return _database.HashDeleteAsync(RedisKeys.Rooms.RoomList(conferenceId), roomIds);
        }

        public Task<Dictionary<string, Room>> GetAll(string conferenceId)
        {
            return _database.HashGetAllAsync<Room>(RedisKeys.Rooms.RoomList(conferenceId));
        }

        public Task<Room> Get(string conferenceId, string roomId)
        {
            return _database.HashGetAsync<Room>(RedisKeys.Rooms.RoomList(conferenceId), roomId);
        }

        public Task DeleteAll(string conferenceId)
        {
            return _database.RemoveAsync(RedisKeys.Rooms.RoomList(conferenceId));
        }

        public Task<Dictionary<string, string>> GetParticipantRooms(string conferenceId)
        {
            return _database.HashGetAllAsync<string>(RedisKeys.Rooms.ParticipantsToRoom(conferenceId));
        }

        public Task UnsetParticipantRoom(string conferenceId, string participantId)
        {
            return _database.HashDeleteAsync(RedisKeys.Rooms.ParticipantsToRoom(conferenceId), participantId);
        }

        public Task<string?> GetParticipantRoom(string conferenceId, string participantId)
        {
            return _database.HashGetAsync<string>(RedisKeys.Rooms.ParticipantsToRoom(conferenceId), participantId);
        }

        public Task DeleteParticipantToRoomMap(string conferenceId)
        {
            return _database.RemoveAsync(RedisKeys.Rooms.ParticipantsToRoom(conferenceId));
        }

        public Task<string?> GetDefaultRoomId(string conferenceId)
        {
            return _database.GetAsync<string>(RedisKeys.Rooms.DefaultRoomId(conferenceId));
        }

        public Task<Dictionary<string, JsonElement>> GetRoomPermissions(string conferenceId, string roomId)
        {
            return _database.HashGetAllAsync<JsonElement>(RedisKeys.Rooms.RoomPermissions(conferenceId, roomId));
        }

        public async Task SetParticipantRoom(string conferenceId, string participantId, string roomId)
        {
            await _database.HashSetAsync(RedisKeys.Rooms.ParticipantsToRoom(conferenceId), participantId, roomId);
            await _database.PublishAsync(RedisChannels.RoomSwitchedChannel(conferenceId),
                new ConnectionMessage<object?>(null, new ConnectionMessageMetadata(conferenceId, null, participantId)));
        }
    }
}
