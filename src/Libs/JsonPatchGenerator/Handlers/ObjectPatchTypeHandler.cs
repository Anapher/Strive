using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonPatchGenerator.Handlers
{
    public class ObjectPatchTypeHandler : IPatchTypeHandler
    {
        public bool CanPatch(JToken original, JToken modified)
        {
            return original.Type == JTokenType.Object && modified.Type == JTokenType.Object;
        }

        public void CreatePatch(JToken original, JToken modified, JsonPatchPath path, IPatchContext context)
        {
            var originalObj = (JObject) original;
            var modifiedObj = (JObject) modified;

            var allProperties = new HashSet<string>();
            AddProperties(allProperties, originalObj);
            AddProperties(allProperties, modifiedObj);

            foreach (var propertyName in allProperties)
            {
                var originalValue = GetPropertyValue(originalObj, propertyName);
                var modifiedValue = GetPropertyValue(modifiedObj, propertyName);

                var propertyPath = path.AddSegment(propertyName);
                context.CreatePatch(originalValue, modifiedValue, propertyPath);
            }
        }

        private static void AddProperties(HashSet<string> hashSet, JObject jObject)
        {
            hashSet.UnionWith(jObject.Properties().Select(x => x.Name));
        }

        private static JToken GetPropertyValue(JObject jObject, string propertyName)
        {
            var property = jObject.Property(propertyName);
            return property == null ? JValue.CreateNull() : property.Value;
        }
    }
}