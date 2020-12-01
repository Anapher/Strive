using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis.Extensions.Core;

namespace PaderConference.Extensions
{
    /// <summary>
    ///     System.Text.Json implementation of <see cref="ISerializer" />
    /// </summary>
    public class CamelCaseSystemTextJsonSerializer : ISerializer
    {
        private static readonly JsonSerializerOptions Options;

        static CamelCaseSystemTextJsonSerializer()
        {
            Options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                IgnoreNullValues = true,
                WriteIndented = false,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = {new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)},
            };
        }

        /// <inheritdoc />
        public T Deserialize<T>(byte[] serializedObject)
        {
            return JsonSerializer.Deserialize<T>(serializedObject, Options)!;
        }

        /// <inheritdoc />
        public byte[] Serialize(object item)
        {
            return JsonSerializer.SerializeToUtf8Bytes(item, Options);
        }
    }
}
