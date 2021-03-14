using System;
using System.Threading.Tasks;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Infrastructure.KeyValue.Abstractions;
using PaderConference.Infrastructure.KeyValue.Extensions;

namespace PaderConference.Infrastructure.KeyValue.Repos
{
    public class SynchronizedObjectRepository : ISynchronizedObjectRepository, IKeyValueRepo
    {
        private const string PROPERTY_KEY = "SyncObject";

        private readonly IKeyValueDatabase _database;

        public SynchronizedObjectRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask<object?> Create(string conferenceId, string syncObjId, object newValue, Type type)
        {
            var key = GetKey(conferenceId, syncObjId);
            return await _database.GetSetAsync(key, newValue, type);
        }

        public async ValueTask<object?> Get(string conferenceId, string syncObjId, Type type)
        {
            var key = GetKey(conferenceId, syncObjId);
            return await _database.GetAsync(key, type);
        }

        public async ValueTask Remove(string conferenceId, string syncObjId)
        {
            var key = GetKey(conferenceId, syncObjId);
            await _database.KeyDeleteAsync(key);
        }

        private static string GetKey(string conferenceId, string syncObjId)
        {
            return DatabaseKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ForSecondary(syncObjId)
                .ToString();
        }
    }
}
