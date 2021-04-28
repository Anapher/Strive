using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Services;
using Strive.Core.Services.Scenes.Providers.TalkingStick.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class ScenesTalkingStickRepository : ITalkingStickRepository, IKeyValueRepo
    {
        private const string QUEUE_PROPERTY_KEY = "sceneTalkingStickQueue";
        private const string CURRENTSPEAKER_PROPERTY_KEY = "sceneTalkingStickSpeaker";
        private const string LOCK_PROPERTY_KEY = "sceneTalkingStickLock";

        private readonly IKeyValueDatabase _database;

        public ScenesTalkingStickRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public ValueTask Enqueue(Participant participant, string roomId)
        {
            var queueKey = GetQueueKey(participant.ConferenceId, roomId);
            return _database.ListRightPushAsync(queueKey, participant.Id);
        }

        public async ValueTask RemoveFromQueue(IEnumerable<Participant> participants, string roomId)
        {
            using var transaction = _database.CreateTransaction();

            foreach (var participant in participants)
            {
                var queueKey = GetQueueKey(participant.ConferenceId, roomId);
                _ = transaction.ListRemoveAsync(queueKey, participant.Id);
            }

            await transaction.ExecuteAsync();
        }

        public async ValueTask<Participant?> Dequeue(string conferenceId, string roomId)
        {
            var queueKey = GetQueueKey(conferenceId, roomId);

            var participantId = await _database.ListLeftPopAsync(queueKey);
            return participantId == null ? null : new Participant(conferenceId, participantId);
        }

        public async ValueTask ClearQueue(string conferenceId, string roomId)
        {
            var queueKey = GetQueueKey(conferenceId, roomId);
            await _database.KeyDeleteAsync(queueKey);
        }

        public async ValueTask<IReadOnlyList<string>> FetchQueue(string conferenceId, string roomId)
        {
            var queueKey = GetQueueKey(conferenceId, roomId);
            return await _database.ListRangeAsync(queueKey, 0, -1);
        }

        public async ValueTask SetCurrentSpeakerAndRemoveFromQueue(Participant participant, string roomId)
        {
            var speakerKey = GetSpeakerKey(participant.ConferenceId, roomId);
            var queueKey = GetQueueKey(participant.ConferenceId, roomId);

            using (var transaction = _database.CreateTransaction())
            {
                _ = transaction.ListRemoveAsync(queueKey, participant.Id);
                _ = transaction.SetAsync(speakerKey, participant.Id);

                await transaction.ExecuteAsync();
            }
        }

        public async ValueTask RemoveCurrentSpeaker(string conferenceId, string roomId)
        {
            var speakerKey = GetSpeakerKey(conferenceId, roomId);
            await _database.KeyDeleteAsync(speakerKey);
        }

        public async ValueTask<Participant?> GetCurrentSpeaker(string conferenceId, string roomId)
        {
            var speakerKey = GetSpeakerKey(conferenceId, roomId);
            var participantId = await _database.GetAsync(speakerKey);

            return participantId == null ? null : new Participant(conferenceId, participantId);
        }

        public async ValueTask<IAcquiredLock> LockRoom(string conferenceId, string roomId)
        {
            var lockKey = GetLockKey(conferenceId, roomId);
            return await _database.AcquireLock(lockKey);
        }

        private static string GetQueueKey(string conferenceId, string roomId)
        {
            return DatabaseKeyBuilder.ForProperty(QUEUE_PROPERTY_KEY).ForConference(conferenceId).ForSecondary(roomId)
                .ToString();
        }

        private static string GetSpeakerKey(string conferenceId, string roomId)
        {
            return DatabaseKeyBuilder.ForProperty(CURRENTSPEAKER_PROPERTY_KEY).ForConference(conferenceId)
                .ForSecondary(roomId).ToString();
        }

        private static string GetLockKey(string conferenceId, string roomId)
        {
            return DatabaseKeyBuilder.ForProperty(LOCK_PROPERTY_KEY).ForConference(conferenceId).ForSecondary(roomId)
                .ToString();
        }
    }
}
