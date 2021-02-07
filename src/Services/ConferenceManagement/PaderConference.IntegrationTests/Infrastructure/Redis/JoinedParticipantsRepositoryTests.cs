using System.Threading.Tasks;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Infrastructure.Redis.Repos;
using PaderConference.IntegrationTests._Helpers;
using StackExchange.Redis.Extensions.Core.Abstractions;
using Xunit;

namespace PaderConference.IntegrationTests.Infrastructure.Redis
{
    public class JoinedParticipantsRepositoryTests : IClassFixture<RedisDbConnector>
    {
        private readonly IRedisDatabase _database;
        private readonly IJoinedParticipantsRepository _repository;

        public JoinedParticipantsRepositoryTests(RedisDbConnector connector)
        {
            _database = connector.CreateConnection();
            _repository = new JoinedParticipantsRepository(_database);
        }

        [Fact]
        public async Task RemoveParticipant_ParticipantExists_RemoveAndReturnTrue()
        {
            const string participantId = "123";
            const string conferenceId = "45";

            var participantKey = $"{participantId}:ParticipantToConference";
            var conferenceKey = $"{conferenceId}:ConferenceParticipants";

            // arrange
            await _database.Database.StringSetAsync(participantKey, conferenceId);
            await _database.Database.HashSetAsync(conferenceKey, participantId, "Me");

            // act
            var result = await _repository.RemoveParticipant(participantId);

            // assert
            Assert.Equal(conferenceId, result);

            var currentConferenceId = await _database.GetAsync<string?>(participantKey);
            Assert.Null(currentConferenceId);

            var hashSetEntry = await _database.HashGetAsync<string?>(conferenceKey, participantId);
            Assert.Null(hashSetEntry);
        }

        [Fact]
        public async Task RemoveParticipant_ParticipantDoesNotExist_ReturnFalse()
        {
            const string participantId = "43";
            const string conferenceId = "564";

            var participantKey = $"{participantId}:ParticipantToConference";
            var conferenceKey = $"{conferenceId}:ConferenceParticipants";

            // arrange
            await _database.Database.KeyDeleteAsync(participantKey);
            await _database.Database.KeyDeleteAsync(conferenceKey);

            // act
            var result = await _repository.RemoveParticipant(participantId);

            // assert
            Assert.Null(result);

            var currentConferenceId = await _database.GetAsync<string?>(participantKey);
            Assert.Null(currentConferenceId);

            var hashSetEntry = await _database.HashGetAsync<string?>(conferenceKey, participantId);
            Assert.Null(hashSetEntry);
        }
    }
}
