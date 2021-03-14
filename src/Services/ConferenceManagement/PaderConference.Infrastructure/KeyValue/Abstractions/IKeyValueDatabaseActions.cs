using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Infrastructure.KeyValue.Redis.Scripts;
using StackExchange.Redis;

namespace PaderConference.Infrastructure.KeyValue.Abstractions
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

        ValueTask ListRightPushAsync(string key, string item);

        ValueTask<int> ListLenAsync(string key);

        ValueTask<IReadOnlyList<string>> ListRangeAsync(string key, int start, int end);

        /// <returns>
        ///     <see langword="true" /> if the element is added to the set; <see langword="false" /> if the element is already
        ///     present.
        /// </returns>
        ValueTask<bool> SetAddAsync(string key, string value);

        /// <returns><see langword="true" /> if the element is successfully found and removed; otherwise, <see langword="false" />.</returns>
        ValueTask<bool> SetRemoveAsync(string key, string value);

        ValueTask<IReadOnlyList<string>> SetMembersAsync(string key);

        ValueTask<RedisResult> ExecuteScriptAsync(RedisScript script, params string[] parameters);
    }
}
