using System;
using System.Threading.Tasks;
using Strive.Core.Services.BreakoutRooms;
using Strive.Core.Services.BreakoutRooms.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Extensions;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class BreakoutRoomRepository : IBreakoutRoomRepository, IKeyValueRepo
    {
        private const string PROPERTY_KEY = "BreakoutRooms";
        private const string LOCK_KEY = "BreakoutRoomsLock";

        private readonly IKeyValueDatabase _database;

        public BreakoutRoomRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask<BreakoutRoomInternalState?> Get(string conferenceId)
        {
            var key = GetKey(conferenceId);
            return await _database.GetAsync<BreakoutRoomInternalState?>(key);
        }

        public async ValueTask Set(string conferenceId, BreakoutRoomInternalState state)
        {
            var key = GetKey(conferenceId);
            await _database.SetAsync(key, state);
        }

        public async ValueTask Remove(string conferenceId)
        {
            var key = GetKey(conferenceId);
            await _database.KeyDeleteAsync(key);
        }

        public async ValueTask<IAsyncDisposable> LockBreakoutRooms(string conferenceId)
        {
            var key = GetLockKey(conferenceId);
            return await _database.AcquireLock(key);
        }

        private static string GetKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(PROPERTY_KEY).ForConference(conferenceId).ToString();
        }

        private static string GetLockKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(LOCK_KEY).ForConference(conferenceId).ToString();
        }

        public ValueTask RemoveAllDataOfConference(string conferenceId)
        {
            return Remove(conferenceId);
        }
    }
}
