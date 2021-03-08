using Newtonsoft.Json.Linq;

namespace JsonPatchGenerator
{
    public interface IPatchTypeHandler
    {
        bool CanPatch(JToken original, JToken modified);

        void CreatePatch(JToken original, JToken modified, JsonPatchPath path, IPatchContext context);
    }
}