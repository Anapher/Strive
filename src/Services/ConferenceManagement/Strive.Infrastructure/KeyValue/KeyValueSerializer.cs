using System;
using Newtonsoft.Json;
using Strive.Infrastructure.Serialization;

namespace Strive.Infrastructure.KeyValue
{
    public static class KeyValueSerializer
    {
        private static readonly JsonSerializerSettings Settings = JsonConfig.Default;

        public static string SerializeValue(object? value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public static T? DeserializeValue<T>(string data)
        {
            if (string.IsNullOrEmpty(data)) return default;
            return JsonConvert.DeserializeObject<T>(data, Settings);
        }

        public static object? DeserializeValue(string data, Type type)
        {
            if (string.IsNullOrEmpty(data)) return default;
            return JsonConvert.DeserializeObject(data, type, Settings);
        }
    }
}
