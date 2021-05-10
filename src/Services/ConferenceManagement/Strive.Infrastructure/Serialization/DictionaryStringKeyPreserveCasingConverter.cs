using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Strive.Infrastructure.Serialization
{
    /// <summary>
    ///     Converts string dictionaries (<see cref="IReadOnlyDictionary{TKey,TValue}" /> of string to json while preserving
    ///     the casing of the keys
    /// </summary>
    public class DictionaryStringKeyPreserveCasingConverter : JsonConverter
    {
        public override bool CanRead { get; } = false;
        public override bool CanWrite { get; } = true;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var dictionaryInterface = GetReadOnlyStringDictionaryInterfaceType(value.GetType())!;
            var valueType = dictionaryInterface.GetGenericArguments()[1];

            var method = typeof(DictionaryStringKeyPreserveCasingConverter).GetMethod(nameof(WriteDictionary));
            var generic = method!.MakeGenericMethod(valueType);
            generic.Invoke(null, new[] {value, writer, serializer});
        }

        public static void WriteDictionary<T>(IReadOnlyDictionary<string, T> dictionary, JsonWriter writer,
            JsonSerializer serializer)
        {
            writer.WriteStartObject();
            foreach (var (key, value) in dictionary)
            {
                writer.WritePropertyName(key);
                serializer.Serialize(writer, value);
            }

            writer.WriteEndObject();
        }


        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return GetReadOnlyStringDictionaryInterfaceType(objectType) != null;
        }

        private static Type? GetReadOnlyStringDictionaryInterfaceType(Type givenType)
        {
            var interfaceTypes = givenType.GetInterfaces();
            return interfaceTypes.FirstOrDefault(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>) &&
                x.GetGenericArguments()[0] == typeof(string));
        }
    }
}
