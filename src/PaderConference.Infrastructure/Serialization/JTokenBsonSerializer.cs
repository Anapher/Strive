using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaderConference.Infrastructure.Serialization
{
    public class JTokenBsonSerializer : SerializerBase<JValue?>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JValue? value)
        {
            if (value == null)
                context.Writer.WriteNull();
            else context.Writer.WriteString(value.ToString(Formatting.None));
        }

        public override JValue? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.GetCurrentBsonType();
            if (type == BsonType.String)
            {
                var s = context.Reader.ReadString();
                return (JValue) JToken.Parse(s);
            }

            return null;
        }
    }
}
