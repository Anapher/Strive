using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Infrastructure.Redis.Scripts;
using StackExchange.Redis;

namespace PaderConference.Infrastructure.Redis.Abstractions
{
    public interface IKeyValueDatabaseActions
    {
        ValueTask<bool> KeyDeleteAsync(string key);

        ValueTask<string?> HashGetAsync(string hashKey, string key);

        ValueTask HashSetAsync(string hashKey, IEnumerable<KeyValuePair<string, string>> keyValuePairs);

        ValueTask HashSetAsync(string hashKey, string key, string value);

        ValueTask<bool> HashExists(string hashKey, string key);

        ValueTask<bool> HashDeleteAsync(string hashKey, string key);

        ValueTask<IReadOnlyDictionary<string, string>> HashGetAllAsync(string hashKey);

        ValueTask<string?> GetAsync(string key);

        ValueTask<string?> GetSetAsync(string key, string value);

        ValueTask SetAsync(string key, string value);

        ValueTask<RedisResult> ExecuteScriptAsync(RedisScript script, params object[] parameters);
    }
}
