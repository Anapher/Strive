using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Infrastructure.Redis.Scripts;
using StackExchange.Redis;

namespace PaderConference.Infrastructure.Redis.Abstractions
{
    public interface IKeyValueDatabaseActions
    {
        ValueTask<bool> KeyDeleteAsync(string key);

        ValueTask<string?> HashGetAsync(string key, string field);

        ValueTask HashSetAsync(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs);

        ValueTask HashSetAsync(string key, string field, string value);

        ValueTask<bool> HashExistsAsync(string key, string field);

        ValueTask<bool> HashDeleteAsync(string key, string field);

        ValueTask<IReadOnlyDictionary<string, string>> HashGetAllAsync(string key);

        ValueTask<string?> GetAsync(string key);

        ValueTask<string?> GetSetAsync(string key, string value);

        ValueTask SetAsync(string key, string value);

        ValueTask<RedisResult> ExecuteScriptAsync(RedisScript script, params string[] parameters);
    }
}
