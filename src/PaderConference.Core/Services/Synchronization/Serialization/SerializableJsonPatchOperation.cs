using Microsoft.AspNetCore.JsonPatch.Operations;
using PaderConference.Core.Extensions;

namespace PaderConference.Core.Services.Synchronization.Serialization
{
    public class SerializableJsonPatchOperation
    {
        public SerializableJsonPatchOperation(Operation operation)
        {
            path = operation.path.ToCamelCasePath();
            op = operation.op;
            if (!string.IsNullOrEmpty(operation.from))
                from = operation.from.ToCamelCasePath();
            value = operation.value;
        }

        public object value { get; set; }

        public string path { get; set; }

        public string op { get; set; }

        public string? from { get; set; }
    }
}