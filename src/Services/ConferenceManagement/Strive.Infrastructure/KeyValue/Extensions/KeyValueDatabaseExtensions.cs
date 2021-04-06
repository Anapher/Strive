using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Strive.Infrastructure.KeyValue.Abstractions;

namespace Strive.Infrastructure.KeyValue.Extensions
{
    public static class KeyValueDatabaseExtensions
    {
        public static async ValueTask<T?> HashGetAsync<T>(this IKeyValueDatabaseActions database, string hashKey,
            string key)
        {
            var result = await database.HashGetAsync(hashKey, key);
            if (result == null) return default;

            return KeyValueSerializer.DeserializeValue<T>(result);
        }

        public static ValueTask HashSetAsync<T>(this IKeyValueDatabaseActions database, string hashKey, string key,
            T value)
        {
            var serialized = KeyValueSerializer.SerializeValue(value);
            return database.HashSetAsync(hashKey, key, serialized);
        }

        public static async ValueTask<IReadOnlyDictionary<string, T?>> HashGetAllAsync<T>(
            this IKeyValueDatabaseActions database, string hashKey)
        {
            var result = await database.HashGetAllAsync(hashKey);
            return result.ToDictionary(x => x.Key, x => KeyValueSerializer.DeserializeValue<T>(x.Value));
        }

        public static async ValueTask<T?> GetAsync<T>(this IKeyValueDatabaseActions database, string key)
        {
            var result = await database.GetAsync(key);
            if (result == null) return default;

            return KeyValueSerializer.DeserializeValue<T>(result);
        }

        public static async ValueTask<object?> GetAsync(this IKeyValueDatabaseActions database, string key, Type type)
        {
            var result = await database.GetAsync(key);
            if (result == null) return default;

            return KeyValueSerializer.DeserializeValue(result, type);
        }

        public static ValueTask SetAsync<T>(this IKeyValueDatabaseActions database, string key, T value)
        {
            var serialized = KeyValueSerializer.SerializeValue(value);
            return database.SetAsync(key, serialized);
        }

        public static async ValueTask<T?> GetSetAsync<T>(this IKeyValueDatabaseActions database, string key, T value)
        {
            var serialized = KeyValueSerializer.SerializeValue(value);
            var result = await database.GetSetAsync(key, serialized);
            if (result == null) return default;

            return KeyValueSerializer.DeserializeValue<T>(result);
        }

        public static async ValueTask<object?> GetSetAsync(this IKeyValueDatabaseActions database, string key,
            object value, Type type)
        {
            var serialized = KeyValueSerializer.SerializeValue(value);
            var result = await database.GetSetAsync(key, serialized);
            if (result == null) return default;

            return KeyValueSerializer.DeserializeValue(result, type);
        }

        public static ValueTask ListRightPushAsync(this IKeyValueDatabaseActions database, string key, object item)
        {
            var serialized = KeyValueSerializer.SerializeValue(item);
            return database.ListRightPushAsync(key, serialized);
        }

        public static async ValueTask<IReadOnlyList<T?>> ListRangeAsync<T>(this IKeyValueDatabaseActions database,
            string key, int start, int end)
        {
            var result = await database.ListRangeAsync(key, start, end);
            return result.Select(KeyValueSerializer.DeserializeValue<T>).ToList();
        }
    }
}
