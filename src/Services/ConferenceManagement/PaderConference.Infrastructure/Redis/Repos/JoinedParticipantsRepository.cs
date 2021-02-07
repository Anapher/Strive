using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Infrastructure.Redis.Scripts;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class JoinedParticipantsRepository : IJoinedParticipantsRepository, IRedisRepo
    {
        private const string PARTICIPANT_TO_CONFERENCE_KEY = "ParticipantToConference";
        private const string CONFERENCE_TO_PARTICIPANTS_KEY = "ConferenceParticipants";

        private readonly IRedisDatabase _redisDatabase;

        public JoinedParticipantsRepository(IRedisDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        public async Task<string?> RegisterParticipant(string participantId, string conferenceId,
            JoinedParticipantData data)
        {
            var participantToConferenceKey = RedisKeyBuilder.ForProperty(PARTICIPANT_TO_CONFERENCE_KEY)
                .ForParticipant(participantId).ToString();
            var conferenceToParticipantsKey = RedisKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference(conferenceId).ToString();

            var serializedData = RedisSerializer.SerializeValue(data);

            var trans = _redisDatabase.Database.CreateTransaction();
            {
                var previousConferenceIdTask = RemoveParticipant(participantId);

                var _ = trans.StringSetAsync(participantToConferenceKey, conferenceId);
                var __ = trans.HashSetAsync(conferenceToParticipantsKey, participantId, serializedData);

                await trans.ExecuteAsync();

                var previousConferenceId = await previousConferenceIdTask;
                return previousConferenceId;
            }
        }

        public async Task<string?> RemoveParticipant(string participantId)
        {
            return await RemoveParticipant(_redisDatabase.Database, participantId);
        }

        private static async Task<string?> RemoveParticipant(IDatabaseAsync database, string participantId)
        {
            var participantToConferenceKey = RedisKeyBuilder.ForProperty(PARTICIPANT_TO_CONFERENCE_KEY)
                .ForParticipant(participantId).ToString();
            var conferenceToParticipantsKey = RedisKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference("*").ToString();

            var scriptContent = RedisScriptLoader.Load(RedisScript.JoinedParticipantsRepository_RemoveParticipant);
            var result = await database.ScriptEvaluateAsync(scriptContent,
                new RedisKey[] {participantId, participantToConferenceKey, conferenceToParticipantsKey});

            if (result.IsNull) return null;
            return (string) result;
        }

        public async Task<string?> GetConferenceIdOfParticipant(string participantId)
        {
            var key = RedisKeyBuilder.ForProperty(PARTICIPANT_TO_CONFERENCE_KEY).ForParticipant(participantId)
                .ToString();

            return await _redisDatabase.Database.StringGetAsync(key);
        }

        public async Task<IReadOnlyDictionary<string, JoinedParticipantData>> GetParticipantsOfConference(
            string conferenceId)
        {
            var conferenceToParticipantsKey = RedisKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference(conferenceId).ToString();

            return await _redisDatabase.HashGetAllAsync<JoinedParticipantData>(conferenceToParticipantsKey);
        }
    }
}
