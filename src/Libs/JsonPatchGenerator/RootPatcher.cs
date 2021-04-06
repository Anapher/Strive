using System;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;

namespace JsonPatchGenerator
{
    public class RootPatcher : IPatchContext
    {
        public RootPatcher(JsonPatchOptions options)
        {
            Options = options;
        }

        public JsonPatchDocument Document { get; } = new();

        public JsonPatchOptions Options { get; }

        public void CreatePatch(JToken original, JToken modified, JsonPatchPath path)
        {
            if (JToken.DeepEquals(original, modified)) return;

            if (original.Type == JTokenType.Null)
            {
                Document.Add(path.ToString(), modified);
                return;
            }

            if (modified.Type == JTokenType.Null)
            {
                Document.Remove(path.ToString());
                return;
            }

            foreach (var handler in Options.Handlers)
            {
                if (handler.CanPatch(original, modified))
                {
                    handler.CreatePatch(original, modified, path, this);
                    return;
                }
            }

            throw new NotSupportedException();
        }
    }
}