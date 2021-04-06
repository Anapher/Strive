using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Services;
using Strive.Core.Services.ConferenceControl;
using Strive.Core.Services.ParticipantsList.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Extensions;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class ParticipantMetadataRepository : IParticipantMetadataRepository, IKeyValueRepo
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

        public async ValueTask<ParticipantMetadata?> GetParticipantMetadata(Participant participant)
        {
            var key = GetKey(participant.ConferenceId);
            return await _database.HashGetAsync<ParticipantMetadata>(key, participant.Id);
        }

        private static string GetKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
