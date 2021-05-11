using System;
using System.Collections;
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
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }


            var dictionary = (IDictionary) value;
            WriteDictionary(dictionary, writer, serializer);
        }

        public static void WriteDictionary(IDictionary dictionary, JsonWriter writer, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            foreach (var key in dictionary.Keys.Cast<string>())
            {
                writer.WritePropertyName(key);
                serializer.Serialize(writer, dictionary[key]);
            }

            writer.WriteEndObject();
        }


        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override bool CanConvert(Type objectType)
        {
            if (!IsStringDictionary(objectType)) return false;

            if (!objectType.IsAssignableTo(typeof(IDictionary)))
                throw new InvalidOperationException(
                    $"{objectType} is a string dictionary that does not implement IDictionary. For this converter we assume that every IReadOnlyDictionary<string, any> also implements IDictionary, please check this type.");

            return true;
        }

        private static bool IsStringDictionary(Type givenType)
        {
            var interfaceTypes = givenType.GetInterfaces();
            return interfaceTypes.Any(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>) &&
                x.GetGenericArguments()[0] == typeof(string));
        }
    }
}
