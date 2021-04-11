using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Services;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Redis.Scripts;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class JoinedParticipantsRepository : IJoinedParticipantsRepository, IKeyValueRepo
    {
        private const string PARTICIPANT_TO_CONFERENCE_KEY = "ParticipantToConference";
        private const string CONFERENCE_TO_PARTICIPANTS_KEY = "ConferenceParticipants";
        private const string PARTICIPANT_LOCK = "participantJoinLock";

        private readonly IKeyValueDatabase _database;

        public JoinedParticipantsRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask<PreviousParticipantState?> AddParticipant(Participant participant, string connectionId)
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

        public async ValueTask<bool> RemoveParticipant(string participantId, string connectionId)
        {
            return await RemoveParticipantSafe(_database, participantId, connectionId) == true;
        }

        private static async ValueTask<PreviousParticipantState?> RemoveParticipant(IKeyValueDatabaseActions database,
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

        private static async ValueTask<bool?> RemoveParticipantSafe(IKeyValueDatabaseActions database,
            string participantId, string connectionId)
        {
            var participantToConferenceKey = GetParticipantToConferenceKey(participantId);
            var conferenceToParticipantsKey = GetConferenceToParticipantsKey("*");

            var result = await database.ExecuteScriptAsync(
                RedisScript.JoinedParticipantsRepository_RemoveParticipantSafe, participantId,
                participantToConferenceKey, conferenceToParticipantsKey, connectionId);

            if (result.IsNull) return null;
            return (bool) result;
        }

        public async ValueTask<string?> GetConferenceIdOfParticipant(string participantId)
        {
            var key = GetParticipantToConferenceKey(participantId);
            return await _database.GetAsync(key);
        }

        public async ValueTask<IEnumerable<Participant>> GetParticipantsOfConference(string conferenceId)
        {
            var conferenceToParticipantsKey = DatabaseKeyBuilder.ForProperty(CONFERENCE_TO_PARTICIPANTS_KEY)
                .ForConference(conferenceId).ToString();

            return (await _database.HashGetAllAsync(conferenceToParticipantsKey)).Keys.Select(participantId =>
                new Participant(conferenceId, participantId));
        }

        public async ValueTask<bool> IsParticipantJoined(Participant participant)
        {
            return await GetConferenceIdOfParticipant(participant.Id) == participant.ConferenceId;
        }

        public async ValueTask<bool> IsParticipantJoined(Participant participant, string connectionId)
        {
            var conferenceToParticipantsKey = GetConferenceToParticipantsKey(participant.ConferenceId);
            return await _database.HashGetAsync(conferenceToParticipantsKey, participant.Id) == connectionId;
        }

        public async ValueTask<IAcquiredLock> LockParticipantJoin(Participant participant)
        {
            var key = GetParticipantJoinLockKey(participant);
            return await _database.AcquireLock(key);
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

        private static string GetParticipantJoinLockKey(Participant participant)
        {
            return DatabaseKeyBuilder.ForProperty(PARTICIPANT_LOCK).ForConference(participant.ConferenceId)
                .ForSecondary(participant.Id).ToString();
        }
    }
}
