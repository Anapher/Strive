using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Infrastructure.KeyValue.Abstractions;
using PaderConference.Infrastructure.KeyValue.Redis.Scripts;

namespace PaderConference.Infrastructure.KeyValue.Repos
{
    public class JoinedParticipantsRepository : IJoinedParticipantsRepository, IKeyValueRepo
    {
        private const string PARTICIPANT_TO_CONFERENCE_KEY = "ParticipantToConference";
        private const string CONFERENCE_TO_PARTICIPANTS_KEY = "ConferenceParticipants";

        private readonly IKeyValueDatabase _database;

        public JoinedParticipantsRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async Task<PreviousParticipantState?> AddParticipant(Participant participant, string connectionId)
        {
            var participantToConferenceKey = GetParticipantToConferenceKey(participant.Id);
            var conferenceToParticipantsKey = GetConferenceToParticipantsKey(participant.ConferenceId);

            using (var trans = _database.CreateTransaction())
            {
                var previousConferenceIdTask = RemoveParticipant(trans, participant.Id);

                _ = trans.SetAsync(participantToConferenceKey, participant.ConferenceId);
                _ = trans.HashSetAsync(conferenceToParticipantsKey, participant.Id, connectionId);

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
            var participantToConferenceKey = GetParticipantToConferenceKey(participantId);
            var conferenceToParticipantsKey = GetConferenceToParticipantsKey("*");

            var result = await database.ExecuteScriptAsync(RedisScript.JoinedParticipantsRepository_RemoveParticipant,
                participantId, participantToConferenceKey, conferenceToParticipantsKey);

            if (result.IsNull) return null;

            var arr = (string[]) result;
            return new PreviousParticipantState(arr[0], arr[1]);
        }

        private static async Task<bool?> RemoveParticipantSafe(IKeyValueDatabaseActions database, string participantId,
            string connectionId)
        {
            var participantToConferenceKey = GetParticipantToConferenceKey(participantId);
            var conferenceToParticipantsKey = GetConferenceToParticipantsKey("*");

            var result = await database.ExecuteScriptAsync(
                RedisScript.JoinedParticipantsRepository_RemoveParticipantSafe, participantId,
                participantToConferenceKey, conferenceToParticipantsKey, connectionId);

            if (result.IsNull) return null;
            return (bool) result;
        }

        public async Task<string?> GetConferenceIdOfParticipant(string participantId)
        {
            var key = GetParticipantToConferenceKey(participantId);
            return await _database.GetAsync(key);
        }

        public async Task<IEnumerable<Participant>> GetParticipantsOfConference(string conferenceId)
        {
            var conferenceToParticipantsKey = DatabaseKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference(conferenceId).ToString();

            return (await _database.HashGetAllAsync(conferenceToParticipantsKey)).Keys.Select(participantId =>
                new Participant(conferenceId, participantId));
        }

        public async Task<bool> IsParticipantJoined(Participant participant)
        {
            return await GetConferenceIdOfParticipant(participant.Id) == participant.ConferenceId;
        }

        private static string GetParticipantToConferenceKey(string participantId)
        {
            return DatabaseKeyBuilder.ForProperty(PARTICIPANT_TO_CONFERENCE_KEY).ForSecondary(participantId).ToString();
        }

        private static string GetConferenceToParticipantsKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY).ForConference(conferenceId)
                .ToString();
        }
    }
}
