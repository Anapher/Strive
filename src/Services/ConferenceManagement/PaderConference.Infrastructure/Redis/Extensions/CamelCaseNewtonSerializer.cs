using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace PaderConference.Infrastructure.Redis.Extensions
{
    public class CamelCaseNewtonSerializer : NewtonsoftSerializer
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = {new StringEnumConverter(new CamelCaseNamingStrategy())},
        };

        public CamelCaseNewtonSerializer() : base(Settings)
        {
        }
    }
}
