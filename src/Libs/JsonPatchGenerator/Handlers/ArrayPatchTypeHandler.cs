using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonPatchGenerator.Handlers
{
    public class ArrayPatchTypeHandler : IPatchTypeHandler
    {
        public bool CanPatch(JToken original, JToken modified)
        {
            return original.Type == JTokenType.Array && modified.Type == JTokenType.Array;
        }

        public void CreatePatch(JToken original, JToken modified, JsonPatchPath path, IPatchContext context)
        {
            var originalArr = (JArray) original;
            var modifiedArr = (JArray) modified;

            var items = originalArr.ToList();

            // remove all items that were removed
            for (var i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                if (!modifiedArr.Any(x => JToken.DeepEquals(x, item)))
                {
                    context.Document.Remove(path.AtIndex(i).ToString());
                    items.RemoveAt(i);
                }
            }

            // either move or add items that changed
            for (var i = 0; i < modifiedArr.Count; i++)
            {
                var modifiedItem = modifiedArr[i];

                var existingItem = FindIndex(items, token => JToken.DeepEquals(modifiedItem, token));
                if (existingItem > -1)
                {
                    if (existingItem == i) continue;

                    var origItem = items[existingItem];

                    var sourcePath = path.AtIndex(existingItem);
                    var targetPath = path.AtIndex(i);
                    context.Document.Move(sourcePath.ToString(), targetPath.ToString());

                    items.RemoveAt(existingItem);
                    items.Insert(i, origItem);
                }
                else
                {
                    var itemPath = items.Count == i ? path.AddSegment("-") : path.AtIndex(i);
                    context.Document.Add(itemPath.ToString(), modifiedItem);

                    items.Insert(i, modifiedItem);
                }
            }
        }

        private static int FindIndex<T>(IList<T> list, Predicate<T> predicate)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                    return i;
            }

            return -1;
        }
    }
}