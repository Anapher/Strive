using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace PaderConference.Infrastructure.Redis
{
    public static class RedisSerializer
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = {new StringEnumConverter(new CamelCaseNamingStrategy())},
        };

        public static string SerializeValue<T>(T value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public static T? DeserializeValue<T>(string data)
        {
            if (string.IsNullOrEmpty(data)) return default;
            return JsonConvert.DeserializeObject<T>(data, Settings);
        }
    }
}
