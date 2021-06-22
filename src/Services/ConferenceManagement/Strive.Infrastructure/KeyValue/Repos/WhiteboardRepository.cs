using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Services.Whiteboard;
using Strive.Core.Services.Whiteboard.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Extensions;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class WhiteboardRepository : IWhiteboardRepository, IKeyValueRepo
    {
        private const string WHITEBOARD_PROPERTY_KEY = "whiteboards";
        private const string WHITEBOARD_LOCK_KEY = "whiteboardLock";

        private readonly IKeyValueDatabase _database;

        public WhiteboardRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask Create(string conferenceId, string roomId, Whiteboard whiteboard)
        {
            var key = GetRoomKey(conferenceId, roomId);
            await _database.HashSetAsync(key, whiteboard.Id, whiteboard);
        }

        public async ValueTask<IReadOnlyList<Whiteboard>> GetAll(string conferenceId, string roomId)
        {
            var key = GetRoomKey(conferenceId, roomId);
            return (await _database.HashGetAllAsync<Whiteboard>(key)).Values.ToList()!;
        }

        public async ValueTask<Whiteboard?> Get(string conferenceId, string roomId, string whiteboardId)
        {
            var key = GetRoomKey(conferenceId, roomId);
            return await _database.HashGetAsync<Whiteboard>(key, whiteboardId);
        }

        public async ValueTask Delete(string conferenceId, string roomId, string whiteboardId)
        {
            var key = GetRoomKey(conferenceId, roomId);
            await _database.HashDeleteAsync(key, whiteboardId);
        }

        public async ValueTask DeleteAllOfRoom(string conferenceId, string roomId)
        {
            var key = GetRoomKey(conferenceId, roomId);
            await _database.KeyDeleteAsync(key);
        }

        public ValueTask<IAcquiredLock> LockWhiteboard(string conferenceId, string roomId, string whiteboardId)
        {
            var key = GetLockKey(conferenceId, roomId, whiteboardId);
            return _database.AcquireLock(key);
        }

        private static string GetRoomKey(string conferenceId, string roomId)
        {
            return DatabaseKeyBuilder.ForProperty(WHITEBOARD_PROPERTY_KEY).ForConference(conferenceId)
                .ForSecondary(roomId).ToString();
        }

        private static string GetLockKey(string conferenceId, string roomId, string whiteboardId)
        {
            return DatabaseKeyBuilder.ForProperty(WHITEBOARD_LOCK_KEY).ForConference(conferenceId).ForSecondary(roomId)
                .ForSecondary(whiteboardId).ToString();
        }
    }
}
