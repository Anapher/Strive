using System;
using System.Collections;
using System.Collections.Generic;
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
            foreach (var property in CompareObjects(original, modified))
                PatchValue(property.OriginalValue, property.NewValue, patch, $"{path}/{property.Name}");
        }

        private static void PatchValue(object? originalValue, object? newValue, JsonPatchDocument patch, string path)
        {
            if (originalValue == newValue)
                return;

            if (originalValue == null)
            {
                patch.Replace(path, newValue);
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
                patch.Replace(path, newValue);
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
                                if (currentOriginal.Count == i)
                                    patch.Add(path + "/-", newItem.obj);
                                else
                                    patch.Add(path + $"/{i}", newItem.obj);

                                currentOriginal.Insert(i, newItem);
                            }
                        }

                        bool ComparePaths(string pathAdd, string pathRemove)
                        {
                            if (pathAdd == pathRemove) return true;
                            if (pathAdd.EndsWith("-") &&
                                pathRemove.Split('/').Last() == (originalItems.Count - 1).ToString()) return true;

                            return false;
                        }

                        // optimize remove & add on the same index to replace
                        foreach (var addOp in patch.Operations.Where(x =>
                            x.OperationType == OperationType.Add && x.path.StartsWith(path + "/")).ToList())
                        {
                            var removeOp = patch.Operations.FirstOrDefault(x =>
                                ComparePaths(addOp.path, x.path) && x.OperationType == OperationType.Remove);
                            if (removeOp != null)
                            {
                                patch.Operations.Remove(addOp);
                                patch.Operations.Remove(removeOp);
                                patch.Replace(removeOp.path, addOp.value);
                            }
                        }

                        return;
                    }
                    catch (InvalidOperationException)
                    {
                        // just replace
                    }

                // Replace values directly
                patch.Replace(path, newValue);
            }
        }

        private static IEnumerable<ObjectComparisonValue> CompareObjects(object original, object modified)
        {
            var type = original.GetType();
            if (type != modified.GetType())
                throw new ArgumentException("Both objects must have the same type.");

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var originalDict = (IDictionary) original;
                var modifiedDict = (IDictionary) modified;

                var keys = new HashSet<object>(
                    originalDict.Keys.Cast<object>().Concat(modifiedDict.Keys.Cast<object>()));

                foreach (object key in keys)
                {
                    var originalValue = originalDict.Contains(key) ? originalDict[key] : null;
                    var newValue = modifiedDict.Contains(key) ? modifiedDict[key] : null;

                    yield return new ObjectComparisonValue(key.ToString(), originalValue, newValue);
                }
            }
            else
            {
                foreach (var property in type.GetProperties())
                {
                    var originalValue = property.GetValue(original);
                    var newValue = property.GetValue(modified);

                    yield return new ObjectComparisonValue(property.Name, originalValue, newValue);
                }
            }
        }

        private struct ObjectComparisonValue
        {
            public ObjectComparisonValue(string name, object? originalValue, object? newValue)
            {
                Name = name;
                OriginalValue = originalValue;
                NewValue = newValue;
            }

            public string Name { get; }

            public object? OriginalValue { get; }
            public object? NewValue { get; }
        }
    }
}