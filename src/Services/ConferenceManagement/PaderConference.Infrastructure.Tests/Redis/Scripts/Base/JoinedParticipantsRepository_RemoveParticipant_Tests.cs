using System.Threading.Tasks;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Scripts;
using StackExchange.Redis;
using Xunit;

namespace PaderConference.Infrastructure.Tests.Redis.Scripts
{
    public abstract class JoinedParticipantsRepository_RemoveParticipant_Tests
    {
        private readonly IKeyValueDatabase _database;

        public JoinedParticipantsRepository_RemoveParticipant_Tests(IKeyValueDatabase database)
        {
            _database = database;
        }

        private ValueTask<RedisResult> Execute(string participantId, string participantKey,
            string conferenceKeyTemplate)
        {
            return _database.ExecuteScriptAsync(RedisScript.JoinedParticipantsRepository_RemoveParticipant,
                participantId, participantKey, conferenceKeyTemplate);
        }

        [Fact]
        public async Task ParticipantNotJoinedInConference_ReturnNull()
        {
            const string participantKey = "EFAF1C0A-B638-417C-AF23-97B6639C01D4";
            const string participantId = "7EAD907E-3AF3-4DD6-A85F-DEB557345419";
            const string conferenceKeyTemplate = "conference:09f5ecd4b5ee401b828be4c9af2a557c:*";

            // act
            var result = await Execute(participantId, participantKey, conferenceKeyTemplate);

            // assert
            Assert.True(result.IsNull);
        }

        [Fact]
        public async Task ParticipantJoinedInConference_RemoveParticipantAndReturnOldConferenceId()
        {
            const string participantKey = "2AFA88F3-50E3-4356-9529-F47A730B25B0";
            const string participantId = "14FAA2C6-8FAC-46D1-B34C-6916F5D213D7";
            const string conferenceKeyTemplate = "conference:7fa3aba5bfdb4a648294fe9fb5df40dc:*";
            const string currentConferenceId = "50DABA18-7F83-4D0D-8FDD-B2ADE18C5FBC";
            const string currentConferenceKey = "conference:7fa3aba5bfdb4a648294fe9fb5df40dc:" + currentConferenceId;
            const string currentConnectionId = "7996bdfaf063485492974d74e7e3d657";

            // arrange
            await _database.SetAsync(participantKey, currentConferenceId);
            await _database.HashSetAsync(currentConferenceKey, participantId, currentConnectionId);

            // act
            var result = await Execute(participantId, participantKey, conferenceKeyTemplate);

            // assert
            var arr = (string[]) result;
            Assert.Equal(arr[0], currentConferenceId);
            Assert.Equal(arr[1], currentConnectionId);

            var participantKeyExists = await _database.KeyDeleteAsync(participantKey);
            Assert.False(participantKeyExists);

            var participantMappingExists = await _database.HashExistsAsync(currentConferenceKey, participantId);
            Assert.False(participantMappingExists);
        }
    }
}
