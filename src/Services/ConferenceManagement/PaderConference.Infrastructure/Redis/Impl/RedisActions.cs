using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Scripts;
using StackExchange.Redis;

namespace PaderConference.Infrastructure.Redis.Impl
{
    public abstract class RedisActions : IKeyValueDatabaseActions
    {
        private readonly IDatabaseAsync _database;

        protected RedisActions(IDatabaseAsync database)
        {
            _database = database;
        }

        public async ValueTask<bool> KeyDeleteAsync(string key)
        {
            return await _database.KeyDeleteAsync(key);
        }

        public async ValueTask<string?> HashGetAsync(string key, string field)
        {
            return await _database.HashGetAsync(key, field);
        }

        public async ValueTask HashSetAsync(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            var hashEntries = keyValuePairs.Select(x => new HashEntry(x.Key, x.Value)).ToArray();
            await _database.HashSetAsync(key, hashEntries);
        }

        public async ValueTask HashSetAsync(string key, string field, string value)
        {
            await _database.HashSetAsync(key, field, value);
        }

        public async ValueTask<bool> HashExistsAsync(string key, string field)
        {
            return await _database.HashExistsAsync(key, field);
        }

        public async ValueTask<bool> HashDeleteAsync(string key, string field)
        {
            return await _database.HashDeleteAsync(key, field);
        }

        public async ValueTask<IReadOnlyDictionary<string, string>> HashGetAllAsync(string key)
        {
            var hashEntries = await _database.HashGetAllAsync(key);
            return hashEntries.ToStringDictionary();
        }

        public async ValueTask<string?> GetAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async ValueTask<string?> GetSetAsync(string key, string value)
        {
            return await _database.StringGetSetAsync(key, value);
        }

        public async ValueTask SetAsync(string key, string value)
        {
            await _database.StringSetAsync(key, value);
        }

        public async ValueTask<RedisResult> ExecuteScriptAsync(RedisScript script, params string[] parameters)
        {
            var scriptContent = RedisScriptLoader.Load(script);
            return await _database.ScriptEvaluateAsync(scriptContent, parameters.Select(x => (RedisKey) x).ToArray());
        }
    }
}
