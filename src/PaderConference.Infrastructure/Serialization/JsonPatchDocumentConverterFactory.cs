using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using PaderConference.Core.Services.Synchronization.Serialization;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace PaderConference.Infrastructure.Serialization
{
    public class JsonPatchDocumentConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType) return false;
            if (typeToConvert.GetGenericTypeDefinition() != typeof(JsonPatchDocument<>)) return false;

            return true;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type modelType = typeToConvert.GetGenericArguments()[0];

            var converter = (JsonConverter?) Activator.CreateInstance(
                typeof(JsonPatchDocumentConverter<>).MakeGenericType(modelType));

            return converter;
        }

        private class JsonPatchDocumentConverter<T> : System.Text.Json.Serialization.JsonConverter<JsonPatchDocument<T>>
            where T : class
        {
            // ReSharper disable once EmptyConstructor
            public JsonPatchDocumentConverter()
            {
            }

            public override JsonPatchDocument<T>? Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                using (var jsonDocument = JsonDocument.ParseValue(ref reader))
                {
                    var jsonObject = jsonDocument.RootElement.GetRawText();
                    return JsonConvert.DeserializeObject<JsonPatchDocument<T>>(jsonObject);
                }
            }

            public override void Write(Utf8JsonWriter writer, JsonPatchDocument<T> value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer,
                    value.Operations.Select(x => new SerializableJsonPatchOperation(x)).ToList(), options);
            }
        }
    }
}
