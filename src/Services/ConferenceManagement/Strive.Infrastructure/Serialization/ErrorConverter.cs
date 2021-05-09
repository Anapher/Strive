using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Strive.Core.Dto;
using Strive.Infrastructure.Extensions;

namespace Strive.Infrastructure.Serialization
{
    public class ErrorConverter : JsonConverter<Error>
    {
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, Error? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var errorObj =
                JObject.FromObject(value, new JsonSerializer {ContractResolver = serializer.ContractResolver});

            var fieldProp = errorObj.Property(nameof(Error.Fields), StringComparison.OrdinalIgnoreCase);
            if (fieldProp is {Value: JObject fields})
            {
                foreach (var field in fields.Properties().ToList())
                {
                    fields.Remove(field.Name);
                    fields[field.Name.ToCamelCase()] = field.Value;
                }
            }

            errorObj.WriteTo(writer);
        }

        public override Error? ReadJson(JsonReader reader, Type objectType, Error? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
