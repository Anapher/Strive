using System.Threading.Tasks;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Extensions;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class SceneRepository : ISceneRepository, IKeyValueRepo
    {
        private const string SCENES_PROPERTY_KEY = "scenes";
        private const string SCENESTATE_PROPERTY_KEY = "sceneState";
        private const string LOCK_PROPERTY_KEY = "scenesLock";

        private readonly IKeyValueDatabase _database;

        public SceneRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask<IAcquiredLock> LockScene(string conferenceId, string roomId)
        {
            var lockKey = GetLockKey(conferenceId, roomId);
            return await _database.AcquireLock(lockKey);
        }

        public async ValueTask SetScene(string conferenceId, string roomId, ActiveScene scene)
        {
            var key = GetSceneKey(conferenceId);
            await _database.HashSetAsync(key, roomId, scene);
        }

        public async ValueTask<ActiveScene?> GetScene(string conferenceId, string roomId)
        {
            var key = GetSceneKey(conferenceId);
            return await _database.HashGetAsync<ActiveScene>(key, roomId);
        }

        public async ValueTask RemoveScene(string conferenceId, string roomId)
        {
            var key = GetSceneKey(conferenceId);
            await _database.HashDeleteAsync(key, roomId);
        }

        public async ValueTask SetSceneState(string conferenceId, string roomId, SceneState state)
        {
            var key = GetSceneStateKey(conferenceId);
            await _database.HashSetAsync(key, roomId, state);
        }

        public async ValueTask<SceneState?> GetSceneState(string conferenceId, string roomId)
        {
            var key = GetSceneStateKey(conferenceId);
            return await _database.HashGetAsync<SceneState>(key, roomId);
        }

        public async ValueTask RemoveSceneState(string conferenceId, string roomId)
        {
            var key = GetSceneStateKey(conferenceId);
            await _database.HashDeleteAsync(key, roomId);
        }

        private static string GetSceneKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(SCENES_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }

        private static string GetSceneStateKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(SCENESTATE_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }

        private static string GetLockKey(string conferenceId, string roomId)
        {
            return DatabaseKeyBuilder.ForProperty(LOCK_PROPERTY_KEY).ForConference(conferenceId).ForSecondary(roomId)
                .ToString();
        }
    }
}
