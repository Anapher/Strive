using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace PaderConference.Infrastructure.Serialization
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            if (stringValue == null) return TimeSpan.Zero;

            return XmlConvert.ToTimeSpan(stringValue); //8601
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            var stringValue = XmlConvert.ToString(value); //8601
            writer.WriteStringValue(stringValue);
        }
    }
}
