using System.Threading.Tasks;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Redis.Scripts;
using StackExchange.Redis;
using Xunit;

namespace Strive.Infrastructure.Tests.KeyValue.Scripts.Base
{
    public abstract class JoinedParticipantsRepository_RemoveParticipantSafe_Tests
    {
        private readonly IKeyValueDatabase _database;

        protected JoinedParticipantsRepository_RemoveParticipantSafe_Tests(IKeyValueDatabase database)
        {
            _database = database;
        }

        private ValueTask<RedisResult> Execute(string participantId, string participantKey,
            string conferenceKeyTemplate, string connectionId)
        {
            return _database.ExecuteScriptAsync(RedisScript.JoinedParticipantsRepository_RemoveParticipantSafe,
                participantId, participantKey, conferenceKeyTemplate, connectionId);
        }

        [Fact]
        public async Task ParticipantNotJoinedInConference_ReturnNull()
        {
            const string participantKey = "EFAF1C0A-B638-417C-AF23-97B6639C01D4";
            const string participantId = "7EAD907E-3AF3-4DD6-A85F-DEB557345419";
            const string conferenceKeyTemplate = "conference:09f5ecd4b5ee401b828be4c9af2a557c:*";
            const string connectionId = "41ace69929ee4805a41232e79be91443";

            // act
            var result = await Execute(participantId, participantKey, conferenceKeyTemplate, connectionId);

            // assert
            Assert.False((bool) result);
        }

        [Fact]
        public async Task ParticipantJoinedButDifferentConnectionId_DontRemoveAndReturnFalse()
        {
            const string participantKey = "2AFA88F3-50E3-4356-9529-F47A730B25B0";
            const string participantId = "14FAA2C6-8FAC-46D1-B34C-6916F5D213D7";
            const string conferenceKeyTemplate = "conference:7fa3aba5bfdb4a648294fe9fb5df40dc:*";
            const string conferenceId = "50DABA18-7F83-4D0D-8FDD-B2ADE18C5FBC";
            const string conferenceKey = "conference:7fa3aba5bfdb4a648294fe9fb5df40dc:" + conferenceId;
            const string connectionId = "7996bdfaf063485492974d74e7e3d657";

            // arrange
            await _database.SetAsync(participantKey, conferenceId);
            await _database.HashSetAsync(conferenceKey, participantId, connectionId);

            // act
            var result = await Execute(participantId, participantKey, conferenceKeyTemplate, "differentId");

            // assert
            Assert.False((bool) result);

            var actualConferenceId = await _database.GetAsync(participantKey);
            Assert.NotNull(actualConferenceId);
        }

        [Fact]
        public async Task ParticipantJoined_RemoveAndReturnTrue()
        {
            const string participantKey = "2AFA88F3-50E3-4356-9529-F47A730B25B0";
            const string participantId = "14FAA2C6-8FAC-46D1-B34C-6916F5D213D7";
            const string conferenceKeyTemplate = "conference:7fa3aba5bfdb4a648294fe9fb5df40dc:*";
            const string conferenceId = "50DABA18-7F83-4D0D-8FDD-B2ADE18C5FBC";
            const string conferenceKey = "conference:7fa3aba5bfdb4a648294fe9fb5df40dc:" + conferenceId;
            const string connectionId = "7996bdfaf063485492974d74e7e3d657";

            // arrange
            await _database.SetAsync(participantKey, conferenceId);
            await _database.HashSetAsync(conferenceKey, participantId, connectionId);

            // act
            var result = await Execute(participantId, participantKey, conferenceKeyTemplate, connectionId);

            // assert
            Assert.True((bool) result);

            var actualConferenceId = await _database.GetAsync(participantKey);
            Assert.Null(actualConferenceId);

            var actualConnectionId = await _database.HashGetAsync(conferenceKey, participantId);
            Assert.Null(actualConnectionId);
        }
    }
}
