using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Scripts;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class JoinedParticipantsRepository : IJoinedParticipantsRepository, IRedisRepo
    {
        private const string PARTICIPANT_TO_CONFERENCE_KEY = "ParticipantToConference";
        private const string CONFERENCE_TO_PARTICIPANTS_KEY = "ConferenceParticipants";

        private readonly IKeyValueDatabase _database;

        public JoinedParticipantsRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async Task<PreviousParticipantState?> AddParticipant(string participantId, string conferenceId,
            string connectionId)
        {
            var participantToConferenceKey = RedisKeyBuilder.ForProperty(PARTICIPANT_TO_CONFERENCE_KEY)
                .ForParticipant(participantId).ToString();
            var conferenceToParticipantsKey = RedisKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference(conferenceId).ToString();

            using (var trans = _database.CreateTransaction())
            {
                var previousConferenceIdTask = RemoveParticipant(trans, participantId);

                _ = trans.SetAsync(participantToConferenceKey, conferenceId);
                _ = trans.HashSetAsync(conferenceToParticipantsKey, participantId, connectionId);

                await trans.ExecuteAsync();

                var previousConferenceId = await previousConferenceIdTask;
                return previousConferenceId;
            }
        }

        public async Task<bool> RemoveParticipant(string participantId, string connectionId)
        {
            return await RemoveParticipantSafe(_database, participantId, connectionId) == true;
        }

        private static async Task<PreviousParticipantState?> RemoveParticipant(IKeyValueDatabaseActions database,
            string participantId)
        {
            var participantToConferenceKey = RedisKeyBuilder.ForProperty(PARTICIPANT_TO_CONFERENCE_KEY)
                .ForParticipant(participantId).ToString();
            var conferenceToParticipantsKey = RedisKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference("*").ToString();

            var result = await database.ExecuteScriptAsync(RedisScript.JoinedParticipantsRepository_RemoveParticipant,
                participantId, participantToConferenceKey, conferenceToParticipantsKey);

            if (result.IsNull) return null;

            var arr = (string[]) result;
            return new PreviousParticipantState(arr[0], arr[1]);
        }

        private static async Task<bool?> RemoveParticipantSafe(IKeyValueDatabaseActions database, string participantId,
            string connectionId)
        {
            var participantToConferenceKey = RedisKeyBuilder.ForProperty(PARTICIPANT_TO_CONFERENCE_KEY)
                .ForParticipant(participantId).ToString();
            var conferenceToParticipantsKey = RedisKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference("*").ToString();

            var result = await database.ExecuteScriptAsync(
                RedisScript.JoinedParticipantsRepository_RemoveParticipantSafe, participantId,
                participantToConferenceKey, conferenceToParticipantsKey, connectionId);

            if (result.IsNull) return null;
            return (bool) result;
        }

        public async Task<string?> GetConferenceIdOfParticipant(string participantId)
        {
            var key = RedisKeyBuilder.ForProperty(PARTICIPANT_TO_CONFERENCE_KEY).ForParticipant(participantId)
                .ToString();

            return await _database.GetAsync(key);
        }

        public async Task<IEnumerable<string>> GetParticipantsOfConference(string conferenceId)
        {
            var conferenceToParticipantsKey = RedisKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference(conferenceId).ToString();

            return (await _database.HashGetAllAsync(conferenceToParticipantsKey)).Keys;
        }

        public async Task<bool> IsParticipantJoined(string participantId, string conferenceId)
        {
            return await GetConferenceIdOfParticipant(participantId) == conferenceId;
        }
    }
}
