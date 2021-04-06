using System.Threading.Tasks;
using Strive.Core.Interfaces.Gateways.Repositories;
using Strive.Core.Services;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Infrastructure.KeyValue.Repos;
using Strive.Infrastructure.Redis.Tests.Redis;
using Strive.IntegrationTests._Helpers;
using StackExchange.Redis;
using Xunit;

namespace Strive.IntegrationTests.Infrastructure.Redis
{
    public class RoomRepositoryTests : IClassFixture<RedisDbConnector>
    {
        private readonly IDatabase _database;
        private readonly IRoomRepository _repository;

        public RoomRepositoryTests(RedisDbConnector connector)
        {
            var database = connector.CreateConnection();
            _repository = new RoomRepository(KeyValueDatabaseFactory.Create(database));
            _database = database;
        }

        [Fact]
        public async Task SetParticipantRoom_RoomDoesNotExist_ThrowException()
        {
            const string conferenceId = "7f0b547949744d2095c79ff5c609d0e9";
            const string participantId = "d5658ed2e0ca482d9370023aee486e1e";
            const string roomId = "room1";

            // act
            await Assert.ThrowsAsync<ConcurrencyException>(async () =>
            {
                await _repository.SetParticipantRoom(new Participant(conferenceId, participantId), roomId);
            });

            // assert
            var roomKey = $"{conferenceId}:RoomMapping";
            string? participantRoom = await _database.HashGetAsync(roomKey, participantId);
            Assert.Null(participantRoom);
        }

        [Fact]
        public async Task SetParticipantRoom_RoomDoesExist_SetParticipantRoom()
        {
            const string conferenceId = "76ecd22ecad440df9270a40fce2a5899";
            const string participantId = "68e14e3ce8b54e768e5ccb27831686b4";
            const string roomId = "room1";

            // arrange
            await _repository.CreateRoom(conferenceId, new Room(roomId, "hello"));

            // act
            await _repository.SetParticipantRoom(new Participant(conferenceId, participantId), roomId);

            // assert
            var roomKey = $"{conferenceId}:RoomMapping";
            string participantRoom = await _database.HashGetAsync(roomKey, participantId);
            Assert.Equal(roomId, participantRoom);
        }
    }
}
