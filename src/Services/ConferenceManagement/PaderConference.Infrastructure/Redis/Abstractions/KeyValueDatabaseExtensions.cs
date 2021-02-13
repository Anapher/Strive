using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Redis.Abstractions
{
    public static class KeyValueDatabaseExtensions
    {
        public static async ValueTask<T?> HashGetAsync<T>(this IKeyValueDatabaseActions database, string hashKey,
            string key)
        {
            var result = await database.HashGetAsync(hashKey, key);
            if (result == null) return default;

            return RedisSerializer.DeserializeValue<T>(result);
        }

        public static ValueTask HashSetAsync<T>(this IKeyValueDatabaseActions database, string hashKey, string key,
            T value)
        {
            var serialized = RedisSerializer.SerializeValue(value);
            return database.HashSetAsync(hashKey, key, serialized);
        }

        public static async ValueTask<IReadOnlyDictionary<string, T?>> HashGetAllAsync<T>(
            this IKeyValueDatabaseActions database, string hashKey)
        {
            var result = await database.HashGetAllAsync(hashKey);
            return result.ToDictionary(x => x.Key, x => RedisSerializer.DeserializeValue<T>(x.Value));
        }

        public static async ValueTask<T?> GetAsync<T>(this IKeyValueDatabaseActions database, string key)
        {
            var result = await database.GetAsync(key);
            if (result == null) return default;

            return RedisSerializer.DeserializeValue<T>(result);
        }

        public static ValueTask SetAsync<T>(this IKeyValueDatabaseActions database, string key, T value)
        {
            var serialized = RedisSerializer.SerializeValue(value);
            return database.SetAsync(key, serialized);
        }

        public static async ValueTask<T?> GetSetAsync<T>(this IKeyValueDatabaseActions database, string key, T value)
        {
            var serialized = RedisSerializer.SerializeValue(value);
            var result = await database.GetSetAsync(key, serialized);
            if (result == null) return default;

            return RedisSerializer.DeserializeValue<T>(result);
        }
    }
}
