using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonPatchGenerator
{
    public static class JsonPatchFactory
    {
        public static readonly JsonPatchOptions DefaultOptions = new();

        public static JsonPatchDocument Create(object original, object modified, JsonSerializerSettings jsonSettings,
            JsonPatchOptions options)
        {
            var serializer = JsonSerializer.Create(jsonSettings);

            var originalAsToken = JToken.FromObject(original, serializer);
            var modifiedAsToken = JToken.FromObject(modified, serializer);

            return Create(originalAsToken, modifiedAsToken, options);
        }

        public static JsonPatchDocument Create(JToken original, JToken modified, JsonPatchOptions options)
        {
            var rootPatcher = new RootPatcher(options);
            rootPatcher.CreatePatch(original, modified, JsonPatchPath.Root);

            return rootPatcher.Document;
        }
    }
}