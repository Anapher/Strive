using System.Threading.Tasks;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Infrastructure.Redis.Repos;
using PaderConference.IntegrationTests._Helpers;
using StackExchange.Redis;
using Xunit;

namespace PaderConference.IntegrationTests.Infrastructure.Redis
{
    public class JoinedParticipantsRepositoryTests : IClassFixture<RedisDbConnector>
    {
        private readonly IDatabase _database;
        private readonly IJoinedParticipantsRepository _repository;

        public JoinedParticipantsRepositoryTests(RedisDbConnector connector)
        {
            var redisDb = connector.CreateConnection();
            _repository = new JoinedParticipantsRepository(redisDb);
            _database = redisDb.Database;
        }

        [Fact]
        public async Task RemoveParticipant_ParticipantExists_RemoveAndReturnTrue()
        {
            const string participantId = "6dc6c7f9ca0f487683dd90a6ce0a54af";
            const string conferenceId = "91b241c21b7740e688f3ac3a088c90bb";
            const string connectionId = "6f0120c43b9249458df86610c1999855";

            var participantKey = $"{participantId}:ParticipantToConference";
            var conferenceKey = $"{conferenceId}:ConferenceParticipants";

            // arrange
            await _database.StringSetAsync(participantKey, conferenceId);
            await _database.HashSetAsync(conferenceKey, participantId, connectionId);

            // act
            var result = await _repository.RemoveParticipant(participantId, connectionId);

            // assert
            Assert.True(result);

            string? currentConferenceId = await _database.StringGetAsync(participantKey);
            Assert.Null(currentConferenceId);

            string? hashSetEntry = await _database.HashGetAsync(conferenceKey, participantId);
            Assert.Null(hashSetEntry);
        }

        [Fact]
        public async Task RemoveParticipant_ParticipantDoesNotExist_ReturnFalse()
        {
            const string participantId = "73a1baa1ca154959945d10fe5d4c4976";
            const string conferenceId = "6fa262b0a158433384702b3b34458c51";
            const string connectionId = "4852e2e447554926b3dbc91656936f3c";

            var participantKey = $"{participantId}:ParticipantToConference";
            var conferenceKey = $"{conferenceId}:ConferenceParticipants";

            // arrange
            await _database.KeyDeleteAsync(participantKey);
            await _database.KeyDeleteAsync(conferenceKey);

            // act
            var result = await _repository.RemoveParticipant(participantId, connectionId);

            // assert
            Assert.False(result);

            string? currentConferenceId = await _database.StringGetAsync(participantKey);
            Assert.Null(currentConferenceId);

            string? hashSetEntry = await _database.HashGetAsync(conferenceKey, participantId);
            Assert.Null(hashSetEntry);
        }

        [Fact]
        public async Task RemoveParticipant_ParticipantHasDifferentConnectionId_ReturnFalse()
        {
            const string participantId = "f589c9c23c9b4f5d8c6d956fc7a47cd8";
            const string conferenceId = "2b9c6d1649984c7fa78227c61a181f9a";
            const string savedConnectionId = "98d0c84bf76142fa9efba14f4421a255";
            const string connectionId = "f6e7050dca714aa3abfcb2b81231d76f";

            var participantKey = $"{participantId}:ParticipantToConference";
            var conferenceKey = $"{conferenceId}:ConferenceParticipants";

            // arrange
            await _database.StringSetAsync(participantKey, conferenceId);
            await _database.HashSetAsync(conferenceKey, participantId, savedConnectionId);

            // act
            var result = await _repository.RemoveParticipant(participantId, connectionId);

            // assert
            Assert.False(result);

            string? currentConferenceId = await _database.StringGetAsync(participantKey);
            Assert.NotNull(currentConferenceId);

            string? hashSetEntry = await _database.HashGetAsync(conferenceKey, participantId);
            Assert.Equal(savedConnectionId, hashSetEntry);
        }

        [Fact]
        public async Task AddParticipant_ParticipantDoesNotExist_CreateParticipantAndReturnNull()
        {
            const string participantId = "9103f4d781064c478028c8e26d5c02c0";
            const string conferenceId = "d57e6f6be5024c47ad3901c038d208a1";
            const string connectionId = "4f34017716144873b195624f20a647f7";

            var participantKey = $"{participantId}:ParticipantToConference";
            var conferenceKey = $"{conferenceId}:ConferenceParticipants";

            // arrange
            await _database.KeyDeleteAsync(participantKey);
            await _database.KeyDeleteAsync(conferenceKey);

            // act
            var result = await _repository.AddParticipant(participantId, conferenceId, connectionId);

            // assert
            Assert.Null(result);

            string? actualConferenceId = await _database.StringGetAsync(participantKey);
            Assert.Equal(conferenceId, actualConferenceId);

            string? actualConnectionId = await _database.HashGetAsync(conferenceKey, participantId);
            Assert.Equal(connectionId, actualConnectionId);
        }

        [Fact]
        public async Task AddParticipant_ParticipantDoesExist_CreateParticipantAndReturnOldInfo()
        {
            const string participantId = "f138bfc05be54d2a9bfade96f43c1be1";
            const string conferenceId = "0c091a8837004a259dba7e6244368060";
            const string connectionId = "21baedf90649432f853479dba874aee6";
            const string oldConnectionId = "bde650a1dbeb4416b084c4dfe7d1b4be";
            const string oldConferenceId = "f157f0e6c5e64add8c44951085b35233";

            var participantKey = $"{participantId}:ParticipantToConference";

            var oldConferenceKey = $"{oldConferenceId}:ConferenceParticipants";

            // arrange
            await _database.StringSetAsync(participantKey, oldConferenceId);
            await _database.HashSetAsync(oldConferenceKey, participantId, oldConnectionId);

            // act
            var result = await _repository.AddParticipant(participantId, conferenceId, connectionId);

            // assert
            Assert.NotNull(result);
            Assert.Equal(oldConferenceId, result.ConferenceId);
            Assert.Equal(oldConnectionId, result.ConnectionId);
        }
    }
}
