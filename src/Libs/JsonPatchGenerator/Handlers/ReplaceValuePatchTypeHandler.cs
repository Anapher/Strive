using Newtonsoft.Json.Linq;

namespace JsonPatchGenerator.Handlers
{
    public class ReplaceValuePatchTypeHandler : IPatchTypeHandler
    {
        public bool CanPatch(JToken original, JToken modified)
        {
            return true;
        }

        public void CreatePatch(JToken original, JToken modified, JsonPatchPath path, IPatchContext context)
        {
            context.Document.Add(path.ToString(), modified);
        }
    }
}