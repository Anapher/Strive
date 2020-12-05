using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaderConference.Infrastructure.Serialization
{
    public class JsonCustomEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        private readonly IReadOnlyDictionary<T, string> _enumValues;
        private readonly IReadOnlyDictionary<string, T> _enumValuesReverse;

        public JsonCustomEnumConverter(IReadOnlyDictionary<T, string> enumValues)
        {
            _enumValues = enumValues;
            _enumValuesReverse = enumValues.ToDictionary(x => x.Value, x => x.Key);
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value == null) return default;

            return _enumValuesReverse[value];
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(_enumValues[value]);
        }
    }
}
