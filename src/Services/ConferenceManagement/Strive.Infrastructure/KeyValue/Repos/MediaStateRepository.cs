using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Strive.Core.Services.Media.Dtos;
using Strive.Core.Services.Media.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Extensions;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class MediaStateRepository : IMediaStateRepository, IKeyValueRepo
    {
        private const string PROPERTY_KEY = "mediaState";

        private readonly IKeyValueDatabase _database;

        public MediaStateRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask Set(string conferenceId, IReadOnlyDictionary<string, ParticipantStreams> value)
        {
            var key = GetKey(conferenceId);

            await _database.SetAsync(key, value);
        }

        public async ValueTask<IReadOnlyDictionary<string, ParticipantStreams>> Get(string conferenceId)
        {
            var key = GetKey(conferenceId);

            var result = await _database.GetAsync<IReadOnlyDictionary<string, ParticipantStreams>>(key);
            if (result == null)
                return ImmutableDictionary<string, ParticipantStreams>.Empty;

            return result;
        }

        public async ValueTask Remove(string conferenceId)
        {
            var key = GetKey(conferenceId);

            await _database.KeyDeleteAsync(key);
        }

        private static string GetKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
