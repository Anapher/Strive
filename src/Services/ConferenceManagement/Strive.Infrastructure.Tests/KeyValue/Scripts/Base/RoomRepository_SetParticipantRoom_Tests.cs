using System.Threading.Tasks;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Redis.Scripts;
using StackExchange.Redis;
using Xunit;

namespace Strive.Infrastructure.Tests.KeyValue.Scripts.Base
{
    public abstract class RoomRepository_SetParticipantRoom_Tests
    {
        private readonly IKeyValueDatabase _database;

        protected RoomRepository_SetParticipantRoom_Tests(IKeyValueDatabase database)
        {
            _database = database;
        }

        private ValueTask<RedisResult> Execute(string roomMappingKey, string roomListKey, string participantId,
            string newRoomId)
        {
            return _database.ExecuteScriptAsync(RedisScript.RoomRepository_SetParticipantRoom, roomMappingKey,
                roomListKey, participantId, newRoomId);
        }

        [Fact]
        public async Task RoomDoesNotExist_DoNotChangeAndReturnFalse()
        {
            const string roomMappingKey = "8D87925C-96DA-4B7A-9442-1938422B87E3";
            const string roomListKey = "B9781B02-A5AA-4B10-AE7D-408CC27FC1E6";
            const string participantId = "EB13BB6A-6693-4644-8578-8A47E21C2DA2";
            const string newRoomId = "3189C8E6-4C85-4B8F-9C50-EEB51E09B338";
            const string currentRoomId = "differentRoomId";

            // arrange
            await _database.HashSetAsync(roomListKey, currentRoomId, "something");
            await _database.HashSetAsync(roomMappingKey, participantId, currentRoomId);

            // act
            var result = await Execute(roomMappingKey, roomListKey, participantId, newRoomId);

            // assert
            Assert.False((bool) result);

            var actualMapping = await _database.HashGetAsync(roomMappingKey, participantId);
            Assert.Equal(currentRoomId, actualMapping);
        }

        [Fact]
        public async Task RoomExists_ChangeRoomAndReturnTrue()
        {
            const string roomMappingKey = "8D87925C-96DA-4B7A-9442-1938422B87E3";
            const string roomListKey = "B9781B02-A5AA-4B10-AE7D-408CC27FC1E6";
            const string participantId = "EB13BB6A-6693-4644-8578-8A47E21C2DA2";
            const string newRoomId = "3189C8E6-4C85-4B8F-9C50-EEB51E09B338";
            const string currentRoomId = "differentRoomId";

            // arrange
            await _database.HashSetAsync(roomListKey, currentRoomId, "something");
            await _database.HashSetAsync(roomListKey, newRoomId, "something");
            await _database.HashSetAsync(roomMappingKey, participantId, currentRoomId);

            // act
            var result = await Execute(roomMappingKey, roomListKey, participantId, newRoomId);

            // assert
            Assert.True((bool) result);

            var actualMapping = await _database.HashGetAsync(roomMappingKey, participantId);
            Assert.Equal(newRoomId, actualMapping);
        }
    }
}
