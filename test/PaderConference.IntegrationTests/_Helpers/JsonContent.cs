using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace PaderConference.IntegrationTests._Helpers
{
    public class JsonContent : StringContent
    {
        public JsonContent(object value, JsonSerializer serializer = null) : base(SerializeJson(value, serializer),
            Encoding.UTF8, "application/json")
        {
        }

        private static string SerializeJson(object value, JsonSerializer serializer = null)
        {
            if (serializer == null) return JsonConvert.SerializeObject(value);

            using var writer = new StringWriter();
            serializer.Serialize(writer, value);
            return writer.ToString();
        }
    }
}