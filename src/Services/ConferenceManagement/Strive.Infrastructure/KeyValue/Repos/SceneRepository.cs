using System.Collections.Generic;
using System.Linq;
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
        private const string AVAILABLESCENES_PROPERTY_KEY = "availableScenes";

        private readonly IKeyValueDatabase _database;

        public SceneRepository(IKeyValueDatabase database)
        {
            _database = database;
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

        public async ValueTask SetAvailableScenes(string conferenceId, string roomId, IReadOnlyList<IScene> scenes)
        {
            var key = GetAvailableScenesKey(conferenceId);
            await _database.HashSetAsync(key, roomId, scenes.OrderBy(x => x.ToString()).ToList());
        }

        public async ValueTask<IReadOnlyList<IScene>?> GetAvailableScenes(string conferenceId, string roomId)
        {
            var key = GetAvailableScenesKey(conferenceId);
            return await _database.HashGetAsync<IReadOnlyList<IScene>>(key, roomId);
        }

        public async ValueTask RemoveAvailableScenes(string conferenceId, string roomId)
        {
            var key = GetAvailableScenesKey(conferenceId);
            await _database.HashDeleteAsync(key, roomId);
        }

        private static string GetSceneKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(SCENES_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }

        private static string GetAvailableScenesKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(AVAILABLESCENES_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }
    }
}
