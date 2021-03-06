using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaderConference.IntegrationTests._Helpers
{
    public class ReadOnlyDictionaryConverter : JsonConverter
    {
        public override bool CanRead { get; } = true;
        public override bool CanWrite { get; } = false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            var itemType = objectType.GetGenericArguments();
            var listType = typeof(Dictionary<,>).MakeGenericType(itemType);

            return serializer.Deserialize(reader, listType);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsConstructedGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>);
        }
    }
}
