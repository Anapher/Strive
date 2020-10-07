using System;
using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonPatchGenerator
{
    public static class JsonPatchFactory
    {
        /// <summary>
        ///     Create patch with changes by comparing two objects
        /// </summary>
        /// <param name="original">The original object</param>
        /// <param name="modified">The modified object</param>
        /// <returns>Return json patch with changes to update the <see cref="original" /> to <see cref="modified" /></returns>
        public static JsonPatchDocument CreatePatch(object original, object modified)
        {
            var patch = new JsonPatchDocument();
            PatchValue(original, modified, patch, "");

            return patch;
        }

        private static void FillPatchForObject(object original, object modified, JsonPatchDocument patch, string path)
        {
            var type = original.GetType();
            if (type != modified.GetType())
                throw new ArgumentException("Both objects must have the same type.");

            foreach (var property in type.GetProperties())
            {
                var originalValue = property.GetValue(original);
                var newValue = property.GetValue(modified);

                PatchValue(originalValue, newValue, patch, $"{path}/{property.Name}");
            }
        }

        private static void PatchValue(object originalValue, object newValue, JsonPatchDocument patch, string path)
        {
            if (originalValue == newValue)
                return;

            if (originalValue == null)
            {
                patch.Replace(path, JToken.FromObject(newValue));
                return;
            }

            if (newValue == null)
            {
                patch.Remove(path);
                return;
            }

            var originalObject = JToken.FromObject(originalValue);
            var newObject = JToken.FromObject(newValue);

            if (originalObject.Type != newObject.Type)
            {
                patch.Replace(path, newObject);
            }
            else if (!string.Equals(originalObject.ToString(Formatting.None), newObject.ToString(Formatting.None)))
            {
                if (originalObject.Type == JTokenType.Object)
                {
                    FillPatchForObject(originalValue, newValue, patch, path);
                    return;
                }

                if (originalObject.Type == JTokenType.Array)
                    try
                    {
                        var originalItems = ((IEnumerable) originalValue).Cast<object>()
                            .Select(obj => (obj, JToken.FromObject(obj).ToString(Formatting.None))).ToList();

                        var newItems = ((IEnumerable) newValue).Cast<object>()
                            .Select(obj => (obj, JToken.FromObject(obj).ToString(Formatting.None))).ToList();

                        var currentOriginal = originalItems.ToList();

                        // remove all items that were removed
                        for (var i = originalItems.Count - 1; i >= 0; i--)
                            if (!newItems.Any(x => x.Item2.Equals(originalItems[i].Item2)))
                            {
                                patch.Remove(path + $"/{i}");
                                currentOriginal.RemoveAt(i);
                            }

                        // either move or add items that changed
                        for (var i = 0; i < newItems.Count; i++)
                        {
                            var newItem = newItems[i];

                            var existingItem = currentOriginal.FindIndex(x => x.Item2.Equals(newItem.Item2));
                            if (existingItem > -1)
                            {
                                if (existingItem == i) continue;

                                var origItem = currentOriginal[existingItem];

                                patch.Move(path + $"/{existingItem}", path + $"/{i}");
                                currentOriginal.Remove(origItem);
                                currentOriginal.Insert(i, origItem);
                            }
                            else
                            {
                                patch.Add(path + $"/{i}", JToken.FromObject(newItem.obj));
                                currentOriginal.Insert(i, newItem);
                            }
                        }

                        // optimize remove & add on the same index to replace
                        foreach (var addOp in patch.Operations.Where(x =>
                            x.OperationType == OperationType.Add && x.path.StartsWith(path + "/")).ToList())
                        {
                            var removeOp = patch.Operations.FirstOrDefault(x =>
                                x.path == addOp.path && x.OperationType == OperationType.Remove);
                            if (removeOp != null)
                            {
                                patch.Operations.Remove(addOp);
                                patch.Operations.Remove(removeOp);
                                patch.Replace(addOp.path, addOp.value);
                            }
                        }

                        return;
                    }
                    catch (InvalidOperationException)
                    {
                        // just replace
                    }

                // Replace values directly
                patch.Replace(path, newObject);
            }
        }
    }
}