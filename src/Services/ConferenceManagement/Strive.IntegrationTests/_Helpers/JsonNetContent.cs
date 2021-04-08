using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Strive.Infrastructure.Serialization;

namespace Strive.IntegrationTests._Helpers
{
    public class JsonNetContent : StringContent
    {
        public JsonNetContent(object value, JsonSerializer? serializer = null) : base(SerializeJson(value, serializer),
            Encoding.UTF8, "application/json")
        {
        }

        public static JsonNetContent Create(object value)
        {
            return new(value);
        }

        private static string SerializeJson(object value, JsonSerializer? serializer = null)
        {
            if (serializer == null) return JsonConvert.SerializeObject(value, JsonConfig.Default);

            using var writer = new StringWriter();
            serializer.Serialize(writer, value);
            return writer.ToString();
        }
    }
}
