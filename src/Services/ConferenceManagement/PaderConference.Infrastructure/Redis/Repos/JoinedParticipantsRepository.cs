using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<string?> AddParticipant(string participantId, string conferenceId, string connectionId)
        {
            var participantToConferenceKey = RedisKeyBuilder.ForProperty(PARTICIPANT_TO_CONFERENCE_KEY)
                .ForParticipant(participantId).ToString();
            var conferenceToParticipantsKey = RedisKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference(conferenceId).ToString();

            var trans = _redisDatabase.Database.CreateTransaction();
            {
                var previousConferenceIdTask = RemoveParticipant(trans, participantId);

                _ = trans.StringSetAsync(participantToConferenceKey, conferenceId);
                _ = trans.HashSetAsync(conferenceToParticipantsKey, participantId, connectionId);

                await trans.ExecuteAsync();

                var previousConferenceId = await previousConferenceIdTask;
                return previousConferenceId;
            }
        }

        public async Task<bool> RemoveParticipant(string participantId, string connectionId)
        {
            return await RemoveParticipantSafe(_redisDatabase.Database, participantId, connectionId) == true;
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

        private static async Task<bool?> RemoveParticipantSafe(IDatabaseAsync database, string participantId,
            string connectionId)
        {
            var participantToConferenceKey = RedisKeyBuilder.ForProperty(PARTICIPANT_TO_CONFERENCE_KEY)
                .ForParticipant(participantId).ToString();
            var conferenceToParticipantsKey = RedisKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference("*").ToString();

            var scriptContent = RedisScriptLoader.Load(RedisScript.JoinedParticipantsRepository_RemoveParticipantSafe);
            var result = await database.ScriptEvaluateAsync(scriptContent,
                new RedisKey[] {participantId, participantToConferenceKey, conferenceToParticipantsKey, connectionId});

            if (result.IsNull) return null;
            return (bool) result;
        }

        public async Task<string?> GetConferenceIdOfParticipant(string participantId)
        {
            var key = RedisKeyBuilder.ForProperty(PARTICIPANT_TO_CONFERENCE_KEY).ForParticipant(participantId)
                .ToString();

            return await _redisDatabase.Database.StringGetAsync(key);
        }

        public async Task<IEnumerable<string>> GetParticipantsOfConference(string conferenceId)
        {
            var conferenceToParticipantsKey = RedisKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference(conferenceId).ToString();

            return (await _redisDatabase.HashGetAllAsync<string>(conferenceToParticipantsKey)).Keys;
        }
    }
}
