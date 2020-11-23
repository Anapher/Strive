using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaderConference.Infrastructure.Serialization
{
    public class TypeDiscriminatorConverter<T> : JsonConverter<T> where T : class
    {
        private readonly Dictionary<string, Type> _typeMap;
        private readonly string _typeDiscriminatorPropName;

        public TypeDiscriminatorConverter(Dictionary<string, Type> typeMap, string typeDiscriminatorPropName = "type")
        {
            _typeMap = typeMap;
            _typeDiscriminatorPropName = typeDiscriminatorPropName;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            using (var jsonDocument = JsonDocument.ParseValue(ref reader))
            {
                if (!jsonDocument.RootElement.TryGetProperty(_typeDiscriminatorPropName, out var typeProperty))
                    throw new JsonException();

                if (!_typeMap.TryGetValue(typeProperty.GetString(), out var type))
                    throw new JsonException("Couldn't find type.");

                var jsonObject = jsonDocument.RootElement.GetRawText();
                return (T) JsonSerializer.Deserialize(jsonObject, type, options);
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object) value, options);
        }
    }
}
