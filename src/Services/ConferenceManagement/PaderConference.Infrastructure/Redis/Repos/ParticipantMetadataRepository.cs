using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.ParticipantsList.Gateways;
using PaderConference.Infrastructure.Redis.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class ParticipantMetadataRepository : IParticipantMetadataRepository, IRedisRepo
    {
        private const string PROPERTY_KEY = "participantData";

        private readonly IKeyValueDatabase _database;

        public ParticipantMetadataRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public ValueTask<IReadOnlyDictionary<string, ParticipantMetadata>> GetParticipantsOfConference(
            string conferenceId)
        {
            var key = GetKey(conferenceId);
            return _database.HashGetAllAsync<ParticipantMetadata>(key)!;
        }

        public async ValueTask AddParticipant(Participant participant, ParticipantMetadata data)
        {
            var key = GetKey(participant.ConferenceId);
            await _database.HashSetAsync(key, participant.Id, data);
        }

        public async ValueTask RemoveParticipant(Participant participant)
        {
            var key = GetKey(participant.ConferenceId);
            await _database.HashDeleteAsync(key, participant.Id);
        }

        private static string GetKey(string conferenceId)
        {
            return RedisKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
