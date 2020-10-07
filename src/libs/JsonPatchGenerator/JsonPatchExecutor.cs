//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.AspNetCore.JsonPatch;
//using Microsoft.AspNetCore.JsonPatch.Operations;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json.Serialization;

//namespace JsonPatchGenerator
//{
//    public static class JsonPatchExecutor
//    {
//        public static void ApplyPatch(JsonPatchDocument patch, object objectToApplyTo)
//        {
//            InternalApplyPatch(patch.Operations.ToList(), patch.ContractResolver, objectToApplyTo);
//        }

//        private static void InternalApplyPatch(List<Operation> operations, IContractResolver contractResolver, object objectToApplyTo)
//        {
//            ApplyListOperations(objectToApplyTo, "", operations, contractResolver);

//            var patch = new JsonPatchDocument(operations, contractResolver);
//            patch.ApplyTo(objectToApplyTo);
//        }

//        /// <summary>
//        ///     Search the <see cref="objectToApplyTo" /> for list operations, apply them and remove them from the
//        ///     <see cref="operations" /> list
//        /// </summary>
//        /// <param name="objectToApplyTo">The object the operations should be applied to</param>
//        /// <param name="path">The current path</param>
//        /// <param name="operations">The patch operations. Warning: this list may be modified</param>
//        /// <param name="contractResolver">The contract resolver of the json patch</param>
//        private static void ApplyListOperations(object objectToApplyTo, string path, List<Operation> operations, IContractResolver contractResolver)
//        {
//            foreach (var property in objectToApplyTo.GetType().GetProperties())
//            {
//                if (typeof(IList).IsAssignableFrom(property.PropertyType))
//                {
//                    var propertyPath = $"{path}/{property.Name}";
//                    var ops = operations.Where(x => x.path.StartsWith(propertyPath + "/") || x.path == propertyPath)
//                        .ToList(); // operations targeting the current list

//                    var itemOperations = new Dictionary<string, List<Operation>>();
//                    var list = (IList) property.GetValue(objectToApplyTo);

//                    foreach (var operation in ops)
//                    {
//                        // because we execute it now
//                        operations.Remove(operation);

//                        var objectKey = operation.path.Substring(propertyPath.Length).Split(new[] {'/'}, 2, StringSplitOptions.RemoveEmptyEntries)
//                            .FirstOrDefault() ?? string.Empty;

//                        if (operation.path == propertyPath || operation.path.Length == propertyPath.Length + 1 + objectKey.Length)
//                        {
//                            // the operation is directly on the list property and not on an item
//                            if (operation.OperationType == OperationType.Replace || operation.OperationType == OperationType.Remove)
//                            {
//                                var objToDelete = list.Cast<object>().FirstOrDefault(x => x.GetKey() == objectKey);
//                                if (objToDelete != null)
//                                    list.Remove(objToDelete);
//                            }

//                            if (operation.OperationType == OperationType.Replace || operation.OperationType == OperationType.Add)
//                            {
//                                var itemType = property.PropertyType.GetInterfaces()
//                                    .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GetGenericArguments().First();

//                                var token = (JToken) operation.value;
//                                list.Add(token.ToObject(itemType));
//                            }
//                        }
//                        else
//                        {
//                            if (!itemOperations.TryGetValue(objectKey, out var itemOps))
//                                itemOperations.Add(objectKey, itemOps = new List<Operation>());

//                            var newPath = operation.path.Substring(propertyPath.Length + objectKey.Length + 1);
//                            itemOps.Add(new Operation(operation.op, newPath, operation.from, operation.value));
//                        }
//                    }

//                    foreach (var entryOperation in itemOperations)
//                    {
//                        var entry = list.Cast<object>().First(x => x.GetKey() == entryOperation.Key);
//                        InternalApplyPatch(entryOperation.Value, contractResolver, entry);
//                    }
//                }
//                else
//                {
//                    var value = property.GetValue(objectToApplyTo);
//                    var token = JToken.FromObject(value);
//                    if (token.Type == JTokenType.Object) ApplyListOperations(value, $"{path}/{property.Name}", operations, contractResolver);
//                }

//                if (!operations.Any()) return;
//            }
//        }
//    }
//}

